//-----------------------------------------------------------------------
// <copyright file="UnityGameClientSubsystem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Networking
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class UnityGameClientSubsystem : IGameClientSubsystem
    {
        private static readonly NetworkReader Reader = new NetworkReader(new byte[0]);

        // private static readonly NetworkIdentityUpdate updateNetworkIdentityCache = new NetworkIdentityUpdate();
        private static Dictionary<string, NetworkIdentity> resourcePrefabCache = new Dictionary<string, NetworkIdentity>();

        // Dynamic Network Object Tracking
        private Dictionary<long, NetworkIdentity> dynamicNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
        private List<NetworkIdentity> dynamicNetworkObjectsList = new List<NetworkIdentity>();

        private Dictionary<long, NetworkIdentity> sceneNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
        private List<NetworkIdentity> sceneNetworkObjectsList = new List<NetworkIdentity>();
        private List<NetworkIdentity> onSceneLoadIdentitiesCache = new List<NetworkIdentity>();
        private GameClient gameClient;
        private bool isConnected;

        public void Initialize(GameClient gameClient)
        {
            this.gameClient = gameClient;
            this.gameClient.RegisterMessage<NetworkIdentityRequestUpdate>();
            this.gameClient.RegisterMessage<NetworkIdentityUpdate>();
            this.gameClient.RegisterMessage<NetworkIdentitiesDestroyed>();
            this.gameClient.RegisterMessage<NetworkBehaviourMessage>();
            this.gameClient.RegisterMessage<NetworkBehaviourDataMessage>();
            this.gameClient.RegisterMessage<NetworkIdentityOwnershipRequest>();
            this.gameClient.RegisterMessage<NetworkIdentityReleaseOwnershipRequest>();
        }

        public void Start()
        {
            this.gameClient.ClientReceivedMessage += this.ClientReceivedMessage;
            this.gameClient.ClientConnectedToServer += this.ClientConnected;
            this.gameClient.ClientDisconnectedFromServer += this.ClientDisconnected;

            this.sceneNetworkObjectsList.Clear();
            this.sceneNetworkObjectsHash.Clear();

            // Collecting all the scene network objects from every scene
            NetworkIdentity.GetAllSceneNetworkIdentities(this.sceneNetworkObjectsList);

            // Hashing all the scene network objects for quicker lookup
            foreach (var identity in this.sceneNetworkObjectsList)
            {
                this.AddSceneNetworkIdentityToHash(identity);
            }

            // Making sure we don't miss any if there's a scene load
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        public void Stop()
        {
            this.gameClient.ClientReceivedMessage -= this.ClientReceivedMessage;
            this.gameClient.ClientConnectedToServer -= this.ClientConnected;
            this.gameClient.ClientDisconnectedFromServer -= this.ClientDisconnected;

            this.sceneNetworkObjectsList.Clear();
            this.sceneNetworkObjectsHash.Clear();

            if (Platform.IsApplicationQuitting == false)
            {
                // Destroy all dynamic objects
                foreach (var dynamic in this.dynamicNetworkObjectsList)
                {
                    if (dynamic && dynamic.gameObject)
                    {
                        Pooler.Destroy(dynamic.gameObject);
                    }
                }

                this.dynamicNetworkObjectsList.Clear();
                this.dynamicNetworkObjectsHash.Clear();
            }

            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        public void ClientReceivedMessage(Message message)
        {
            switch (message.GetId())
            {
                case NetworkIdentityUpdate.Id:
                {
                    var networkIdentityUpdatedMessage = (NetworkIdentityUpdate)message;

                    if (this.sceneNetworkObjectsHash.TryGetValue(networkIdentityUpdatedMessage.NetworkId, out NetworkIdentity sceneIdentity))
                    {
                        if (sceneIdentity)
                        {
                            networkIdentityUpdatedMessage.PopulateNetworkIdentity(sceneIdentity, false);
                        }
                        else
                        {
                            Debug.LogError($"Scene NetworkIdentity {networkIdentityUpdatedMessage.NetworkId} has been destoryed, but still getting updates.");
                        }
                    }
                    else if (this.dynamicNetworkObjectsHash.TryGetValue(networkIdentityUpdatedMessage.NetworkId, out NetworkIdentity dynamicIdentity))
                    {
                        networkIdentityUpdatedMessage.PopulateNetworkIdentity(dynamicIdentity, false);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(networkIdentityUpdatedMessage.ResourceName) == false)
                        {
                            var newIdentity = this.CreateDynamicNetworkIdentity(
                                networkIdentityUpdatedMessage.ResourceName,
                                networkIdentityUpdatedMessage.NetworkId,
                                networkIdentityUpdatedMessage.OwnerId,
                                networkIdentityUpdatedMessage.Position,
                                networkIdentityUpdatedMessage.Rotation);

                            networkIdentityUpdatedMessage.PopulateNetworkIdentity(newIdentity, true);
                        }
                    }

                    break;
                }

                case NetworkBehaviourMessage.Id:
                {
                    var networkBehaviourMessage = (NetworkBehaviourMessage)message;

                    NetworkIdentity identity = this.GetNetworkIdentity(networkBehaviourMessage.NetworkId);

                    if (identity && identity.IsOwner == false)
                    {
                        Reader.Replace(networkBehaviourMessage.DataBytes);
                        identity.Behaviours[networkBehaviourMessage.BehaviourIndex].Deserialize(Reader);
                    }

                    break;
                }

                case NetworkBehaviourDataMessage.Id:
                {
                    var networkBehaviourDataMessage = (NetworkBehaviourDataMessage)message;

                    NetworkIdentity identity = this.GetNetworkIdentity(networkBehaviourDataMessage.NetworkId);

                    if (identity)
                    {
                        identity.Behaviours[networkBehaviourDataMessage.BehaviourIndex].OnReceiveNetworkData(networkBehaviourDataMessage.DataKey, networkBehaviourDataMessage.DataValue);
                    }

                    break;
                }

                case NetworkIdentitiesDestroyed.Id:
                {
                    var networkIdentitiesDestoryed = (NetworkIdentitiesDestroyed)message;

                    foreach (long networkId in networkIdentitiesDestoryed.DestroyedNetworkIds)
                    {
                        NetworkIdentity identity = this.GetNetworkIdentity(networkId);

                        if (identity)
                        {
                            Pooler.Destroy(identity.gameObject);
                        }
                    }

                    break;
                }
            }
        }

        public void ClientConnected()
        {
            if (this.gameClient.PrintDebugOutput)
            {
                Debug.Log("UnityGameClientSubsystem: Client Connected");
            }

            this.isConnected = true;

            foreach (var sceneObject in this.sceneNetworkObjectsList)
            {
                sceneObject.RequestUpdate();
            }

            foreach (var dynamicObject in this.dynamicNetworkObjectsList)
            {
                dynamicObject.RequestUpdate();
            }
        }

        public void ClientDisconnected()
        {
            if (this.gameClient.PrintDebugOutput)
            {
                Debug.Log("UnityGameClientSubsystem: Client Disconnected");
            }

            this.isConnected = false;
        }

        public NetworkIdentity CreateDynamicNetworkIdentity(string resourceName, long networkId, long ownerId, Vector3 position, Quaternion rotation)
        {
            if (resourcePrefabCache.TryGetValue(resourceName, out NetworkIdentity networkIdentityPrefab) == false)
            {
                networkIdentityPrefab = Resources.Load<NetworkIdentity>(resourceName);
                resourcePrefabCache.Add(resourceName, networkIdentityPrefab);
            }

            var newNetworkIdentity = Pooler.Instantiate(networkIdentityPrefab);
            newNetworkIdentity.SetClient(this.gameClient);
            newNetworkIdentity.SetNetworkId(networkId);
            newNetworkIdentity.SetOwner(ownerId, newNetworkIdentity.CanChangeOwner);
            newNetworkIdentity.ResourceName = resourceName;
            newNetworkIdentity.transform.SetPositionAndRotation(position, rotation);

            if (this.isConnected)
            {
                newNetworkIdentity.RequestUpdate();
            }

            // Registering the new dynamic object
            this.dynamicNetworkObjectsHash.Add(newNetworkIdentity.NetworkId, newNetworkIdentity);
            this.dynamicNetworkObjectsList.Add(newNetworkIdentity);

            return newNetworkIdentity;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            this.onSceneLoadIdentitiesCache.Clear();
            NetworkIdentity.GetNetworkIdentitiesFromScene(this.onSceneLoadIdentitiesCache, scene);

            // Adding these newly loaded network identities to our list of identities
            this.sceneNetworkObjectsList.AddRange(this.onSceneLoadIdentitiesCache);

            // Adding these newly loaded network identities to our hash as well
            foreach (var identity in this.onSceneLoadIdentitiesCache)
            {
                this.AddSceneNetworkIdentityToHash(identity);
            }
        }

        private void AddSceneNetworkIdentityToHash(NetworkIdentity identity)
        {
            if (this.sceneNetworkObjectsHash.ContainsKey(identity.NetworkId))
            {
                Debug.LogError($"Multiple Network Identities with same Id: {identity.name} and {this.sceneNetworkObjectsHash[identity.NetworkId].name}");
                return;
            }

            this.sceneNetworkObjectsHash.Add(identity.NetworkId, identity);
            identity.SetClient(this.gameClient);

            if (this.isConnected)
            {
                identity.RequestUpdate();
            }
        }

        private NetworkIdentity GetNetworkIdentity(long networkId)
        {
            bool foundSceneIdentity = this.sceneNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity sceneIdentity);
            bool foundDynamicIdentity = this.dynamicNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity dynamicIdentity);

            return foundSceneIdentity ? sceneIdentity :
                   foundDynamicIdentity ? dynamicIdentity :
                   null;
        }
    }
}

#endif
