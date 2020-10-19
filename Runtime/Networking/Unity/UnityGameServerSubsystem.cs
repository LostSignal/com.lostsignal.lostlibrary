//-----------------------------------------------------------------------
// <copyright file="UnityGameServerSubsystem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    // All Objects have a NetworkIdentity component(rename to just Identity so I can resuse?)
    // All Static objects start with OwnerId = -1
    // At startup Clients ask for state of NetworkIdentity.Id
    // Server keeps track of every latest NetworkBehaviour message?

    // NetworkIdetity.RequestUpdate(string resourceName)
    //   sets the ResourceName and sends the NetworkIdentityRequestUpdate message

    // NetworkManager.Startup()
    //    Grab ALL NetworkIdentities in the scene
    //    Call networkIdentity.RequestUpdate()

    // NetworkManager.Instantiate(string resourceName)
    //   Use addressables to create the object
    //   Give a random NetworkId
    //   Call networkIdentity.RequestUpdate(resourceName)

    // Server

    // NetworkManager.Instantiate
    //    Creates the prefab and calls UpdateWithServer(resourceName);

    // Client
    //     * On Startup collect all Static NetworkIdentities in the scenes
    //     * NetworkIdentities sends NetworkIdentityRequestUpdate on first Update tick
    //     * When receives NetworkIdentityUpdate, it either updates existing on NetworkIdentity or Creates one
    //     * When receives NetworkBehaviourMessage, forwards it onto the NetworkIdentity (if it exists)
    //     * When NetworkIdentity destoryed (and you own it), send NetworkIdentitiesDestroyed message

    public class UnityGameServerSubsystem : IGameServerSubsystem
    {
        private readonly NetworkReader reader = new NetworkReader(new byte[0]);
        private readonly Dictionary<long, UnityNetworkObject> sceneUnityNetworkObjects = new Dictionary<long, UnityNetworkObject>();
        private readonly HashSet<long> destroyedSceneUnityNetworkObjects = new HashSet<long>();
        private readonly Dictionary<long, UnityNetworkObject> dynamicUnityGameObjects = new Dictionary<long, UnityNetworkObject>();

        // Message Cache
        private readonly NetworkIdentityUpdate networkIdentityUpdate = new NetworkIdentityUpdate();
        private readonly NetworkBehaviourMessage networkBehaviourMessage = new NetworkBehaviourMessage();
        private readonly NetworkIdentitiesDestroyed networkBehaviourDestoryed = new NetworkIdentitiesDestroyed();

        private GameServer gameServer;

        public void Initialize(GameServer gameServer)
        {
            this.gameServer = gameServer;
            this.gameServer.RegisterMessage<NetworkIdentityRequestUpdate>();
            this.gameServer.RegisterMessage<NetworkIdentityUpdate>();
            this.gameServer.RegisterMessage<NetworkBehaviourMessage>();
            this.gameServer.RegisterMessage<NetworkBehaviourDataMessage>();
            this.gameServer.RegisterMessage<NetworkIdentitiesDestroyed>();
        }

        public Task<bool> Run()
        {
            this.sceneUnityNetworkObjects.Clear();
            this.destroyedSceneUnityNetworkObjects.Clear();
            this.dynamicUnityGameObjects.Clear();

            this.gameServer.ServerReceivedMessage += this.ServerReceivedMessage;
            this.gameServer.ServerUserConnected += this.ServerUserConnected;
            this.gameServer.ServerUserDisconnected += this.ServerUserDisconnected;

            return Task.FromResult<bool>(true);
        }

        public Task Shutdown()
        {
            this.sceneUnityNetworkObjects.Clear();
            this.destroyedSceneUnityNetworkObjects.Clear();
            this.dynamicUnityGameObjects.Clear();

            this.gameServer.ServerReceivedMessage -= this.ServerReceivedMessage;
            this.gameServer.ServerUserConnected -= this.ServerUserConnected;
            this.gameServer.ServerUserDisconnected -= this.ServerUserDisconnected;

            return Task.Delay(0);
        }

        public Task<bool> AllowPlayerToJoin(UserInfo userInfo)
        {
            return Task.FromResult<bool>(true);
        }

        public Task UpdatePlayerInfo(UserInfo userInfo)
        {
            return Task.Delay(0);
        }

        public void ServerReceivedMessage(UserInfo userInfo, Message message, bool reliable)
        {
            switch (message.GetId())
            {
                case NetworkIdentityRequestUpdate.Id:
                {
                    var requestUpdateMessage = (NetworkIdentityRequestUpdate)message;
                    var unityNetworkObject = this.GetObject(requestUpdateMessage.NetworkId);

                    // TODO [bgish]: Check if this is a destroyed scene object and send destroy message instead

                    Debug.Log("NetworkIdentityRequestUpdate " + requestUpdateMessage.NetworkId);

                    if (unityNetworkObject == null)
                    {
                        unityNetworkObject = new UnityNetworkObject
                        {
                            OwnerId = userInfo.UserId,
                            NetworkId = requestUpdateMessage.NetworkId,
                            IsEnabled = requestUpdateMessage.IsEnabled,
                            Position = requestUpdateMessage.Position,
                            Rotation = requestUpdateMessage.Rotation,
                            ResourceName = requestUpdateMessage.ResourceName,
                            BehaviourDatas = new List<BehaviourData>(requestUpdateMessage.BehaviourCount),
                            DestoryOnDisconnect = requestUpdateMessage.DestoryOnDisconnect,
                        };

                        for (int i = 0; i < requestUpdateMessage.BehaviourCount; i++)
                        {
                            unityNetworkObject.BehaviourDatas.Add(new BehaviourData());
                        }

                        bool isScene = string.IsNullOrEmpty(requestUpdateMessage.ResourceName);

                        if (isScene)
                        {
                            this.sceneUnityNetworkObjects.Add(unityNetworkObject.NetworkId, unityNetworkObject);
                        }
                        else
                        {
                            this.dynamicUnityGameObjects.Add(unityNetworkObject.NetworkId, unityNetworkObject);
                        }

                        this.SendIdentityUpdateMessage(unityNetworkObject);
                    }
                    else
                    {
                        this.SendIdentityUpdateMessage(unityNetworkObject, userInfo);

                        // Send all the network behaviour data we know about
                        this.networkBehaviourMessage.NetworkId = unityNetworkObject.NetworkId;

                        for (int i = 0; i < unityNetworkObject.BehaviourDatas.Count; i++)
                        {
                            var behaviourData = unityNetworkObject.BehaviourDatas[i];

                            if (behaviourData?.Data?.Length > 0)
                            {
                                this.networkBehaviourMessage.BehaviourIndex = i;
                                this.networkBehaviourMessage.DataBytes = behaviourData.Data.Bytes;
                                this.networkBehaviourMessage.DataLength = behaviourData.Data.Length;

                                this.gameServer.SendMessageToUser(userInfo, this.networkBehaviourMessage);
                            }
                        }
                    }

                    break;
                }

                case NetworkIdentitiesDestroyed.Id:
                {
                    var destroyedMessage = (NetworkIdentitiesDestroyed)message;

                    if (destroyedMessage.DestroyedNetworkIds?.Count > 0)
                    {
                        for (int i = 0; i < destroyedMessage.DestroyedNetworkIds.Count; i++)
                        {
                            long networkId = destroyedMessage.DestroyedNetworkIds[i];
                            var networkObject = this.GetObject(networkId);

                            if (networkObject != null)
                            {
                                if (networkObject.OwnerId == userInfo.UserId)
                                {
                                    // If static, remove from static dictionary and add to destoryed static list
                                    // If dynamic, just remove from dynamic dictionary
                                    // Delete the object and forward to everyone
                                }
                            }
                        }
                    }

                    // If this came from the owner of the object, then add

                    //

                    // Ignore for now, can't get this till ownership changing exists
                    break;
                }

                case NetworkIdentityUpdate.Id:
                {
                    // Ignore for now, can't get this till ownership changing exists
                    break;
                }

                case NetworkBehaviourMessage.Id:
                {
                    var networkBehaviourMessage = (NetworkBehaviourMessage)message;

                    var networkObject = this.GetObject(networkBehaviourMessage.NetworkId);

                    if (networkObject != null && networkObject.OwnerId == userInfo.UserId)
                    {
                        if (networkObject.BehaviourDatas[networkBehaviourMessage.BehaviourIndex] == null)
                        {
                            networkObject.BehaviourDatas[networkBehaviourMessage.BehaviourIndex] = new BehaviourData();
                        }

                        var behaviourData = networkObject.BehaviourDatas[networkBehaviourMessage.BehaviourIndex];
                        behaviourData.Data.SetData(networkBehaviourMessage.DataBytes, networkBehaviourMessage.DataLength);

                        this.gameServer.SendMessageToAllExcept(userInfo, message, reliable);
                    }

                    break;
                }

                case NetworkBehaviourDataMessage.Id:
                {
                    var networkBehaviourDataMessage = (NetworkBehaviourDataMessage)message;

                    var networkObject = this.GetObject(networkBehaviourDataMessage.NetworkId);

                    if (networkObject != null)
                    {
                        if (networkBehaviourDataMessage.SendType == BehaviourDataSendType.All)
                        {
                            this.gameServer.SendMessageToAll(networkBehaviourDataMessage);
                        }
                        else if (networkBehaviourDataMessage.SendType == BehaviourDataSendType.Others)
                        {
                            this.gameServer.SendMessageToAllExcept(userInfo, networkBehaviourDataMessage);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                    break;
                }
            }
        }

        public void ServerUserConnected(UserInfo userInfo, bool isReconnect)
        {
            // TODO [bgish]: Tell user about all Dynamic Objects...

            foreach (var dynamic in this.dynamicUnityGameObjects.Values)
            {
                this.SendIdentityUpdateMessage(dynamic, userInfo);

                // TODO [bgish]: Also send behaviour Data
            }

            //// Update(this.staticNetworkObjectsList);
            //// Update(this.dynamicNetworkObjectsList);
            ////
            //// // Telling this user what static network objects have been destroyed
            //// networkIdentitiesDestroyedCache.DestroyedNetworkIds.Clear();
            //// networkIdentitiesDestroyedCache.DestroyedNetworkIds.AddRange(this.destroyedStaticNetworkObjects);
            //// this.gameServer.SendMessageToAll(networkIdentitiesDestroyedCache);
            ////
            //// void Update(List<NetworkIdentity> identities)
            //// {
            ////     for (int identityIndex = 0; identityIndex < identities.Count; identityIndex++)
            ////     {
            ////         networkIdentityUpdateCache.PopulateMessage(identities[identityIndex]);
            ////         this.gameServer.SendMessageToUser(userInfo, networkIdentityUpdateCache);
            ////     }
            //// }

            Debug.Log("SERVER: User Connected - " + userInfo.GetDisplayName() + " - " + userInfo.GetPlayFabId());
        }

        public void ServerUserDisconnected(UserInfo userInfo, bool wasConnectionLost)
        {
            // TODO [bgish]: Need to go through every object and reassign any UnityNetworkObject owners from
            //               this owner to a new one, and make sure to tell the remaining clients of the change.
            //               This is kind of a server host migration.

            Debug.Log("SERVER: User Disconnected - " + userInfo.GetDisplayName() + " - " + userInfo.GetPlayFabId());

            this.networkBehaviourDestoryed.DestroyedNetworkIds.Clear();

            foreach (var dynamic in this.dynamicUnityGameObjects.Values)
            {
                if (dynamic.DestoryOnDisconnect && dynamic.OwnerId == userInfo.UserId)
                {
                    this.networkBehaviourDestoryed.DestroyedNetworkIds.Add(dynamic.NetworkId);
                }
            }

            if (this.networkBehaviourDestoryed.DestroyedNetworkIds.Count > 0)
            {
                this.gameServer.SendMessageToAllExcept(userInfo, this.networkBehaviourDestoryed);

                for (int i = 0; i < this.networkBehaviourDestoryed.DestroyedNetworkIds.Count; i++)
                {
                    this.dynamicUnityGameObjects.Remove(this.networkBehaviourDestoryed.DestroyedNetworkIds[i]);
                }
            }
        }

        private void SendIdentityUpdateMessage(UnityNetworkObject unityNetworkObject, UserInfo user = null)
        {
            this.networkIdentityUpdate.NetworkId = unityNetworkObject.NetworkId;
            this.networkIdentityUpdate.OwnerId = unityNetworkObject.OwnerId;
            this.networkIdentityUpdate.IsEnabled = unityNetworkObject.IsEnabled;
            this.networkIdentityUpdate.ResourceName = unityNetworkObject.ResourceName;
            this.networkIdentityUpdate.Position = unityNetworkObject.Position;
            this.networkIdentityUpdate.Rotation = unityNetworkObject.Rotation;

            if (user != null)
            {
                this.gameServer.SendMessageToUser(user, this.networkIdentityUpdate);
            }
            else
            {
                this.gameServer.SendMessageToAll(this.networkIdentityUpdate);
            }
        }

        private UnityNetworkObject GetObject(long networkId)
        {
            if (this.sceneUnityNetworkObjects.TryGetValue(networkId, out UnityNetworkObject sceneObject))
            {
                return sceneObject;
            }
            else if (this.dynamicUnityGameObjects.TryGetValue(networkId, out UnityNetworkObject dynamicObject))
            {
                return dynamicObject;
            }

            return null;
        }

        private class UnityNetworkObject
        {
            public long NetworkId { get; set; }

            public long OwnerId { get; set; }

            public string ResourceName { get; set; }

            public List<BehaviourData> BehaviourDatas { get; set; }

            public Vector3 Position { get; set; }

            public Quaternion Rotation { get; set; }

            public bool IsEnabled { get; set; }

            public bool DestoryOnDisconnect { get; set; }
        }

        private class BehaviourData
        {
            public RawByteArray Data { get; set; } = new RawByteArray();

            public int Version { get; set; }
        }

        private class RawByteArray
        {
            public int Length { get; private set; }

            public byte[] Bytes { get; private set; }

            public void SetData(byte[] newData, int newDataLength)
            {
                if (this.Bytes == null || newDataLength > this.Bytes.Length)
                {
                    this.Bytes = new byte[(int)(newDataLength * 1.5f)];
                }

                this.Length = newDataLength;

                for (int i = 0; i < newDataLength; i++)
                {
                    this.Bytes[i] = newData[i];
                }
            }

            public void SetData(byte[] data)
            {
                this.SetData(data, data.Length);
            }
        }
    }

    /*
    private static readonly NetworkReader reader = new NetworkReader(new byte[0]);

    // Caching Members
    private static readonly NetworkIdentityUpdate networkIdentityUpdateCache = new NetworkIdentityUpdate();
    private static readonly NetworkIdentitiesDestroyed networkIdentitiesDestroyedCache = new NetworkIdentitiesDestroyed();
    private static Dictionary<string, NetworkIdentity> resourcePrefabCache = new Dictionary<string, NetworkIdentity>();

    // Dynamic Network Object Tracking
    private Dictionary<long, NetworkIdentity> dynamicNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
    private List<NetworkIdentity> dynamicNetworkObjectsList = new List<NetworkIdentity>();

    // Static Network Object Tracking
    private Dictionary<long, NetworkIdentity> staticNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
    private List<NetworkIdentity> staticNetworkObjectsList = new List<NetworkIdentity>();
    private HashSet<long> destroyedStaticNetworkObjects = new HashSet<long>();

    NetworkIdentityCreated networkIdentityCreatedMessage = new NetworkIdentityCreated();
    private GameServer gameServer;

    public ReadOnlyCollection<UserInfo> ConnectedUsers => this.gameServer.ConnectedUsers;
    public GameServer GameServer => this.gameServer;

    public UnityGameServer(GameServer gameServer)
    {
        this.StartGameServer(gameServer);
    }

    public void Update()
    {
        this.gameServer?.Update();
    }

    public void Shutdown()
    {
        this.gameServer?.Shutdown();

        if (Platform.IsApplicationQuitting == false)
        {
            // Destroy all dynamic objects
            foreach (var dynamic in this.dynamicNetworkObjectsList)
            {
                Pooler.Destroy(dynamic.gameObject);
            }
        }
    }

    public NetworkIdentity CreateNetworkIdentity(string resourceName, long ownerId, Vector3 position)
    {
        NetworkIdentity networkIdentityPrefab = null;

        if (resourcePrefabCache.TryGetValue(resourceName, out networkIdentityPrefab) == false)
        {
            networkIdentityPrefab = Resources.Load<NetworkIdentity>(resourceName);
            resourcePrefabCache.Add(resourceName, networkIdentityPrefab);
        }

        var newNetworkIdentity = Pooler.Instantiate(networkIdentityPrefab);
        newNetworkIdentity.GenerateNewNetworkId();
        newNetworkIdentity.SetOwner(ownerId);
        newNetworkIdentity.ResourceName = resourceName;
        newNetworkIdentity.Destroyed += this.NetworkIdentityDestroyed;
        newNetworkIdentity.transform.position = position;

        // Registering it with the dynamic game objects
        this.dynamicNetworkObjectsHash.Add(newNetworkIdentity.NetworkId, newNetworkIdentity);
        this.dynamicNetworkObjectsList.Add(newNetworkIdentity);

        this.networkIdentityCreatedMessage.NetworkId = newNetworkIdentity.NetworkId;
        this.networkIdentityCreatedMessage.OwnerId = newNetworkIdentity.OwnerId;
        this.networkIdentityCreatedMessage.ResourceName = resourceName;
        this.networkIdentityCreatedMessage.Position = position;
        this.SendNetworkMessage(this.networkIdentityCreatedMessage);

        return newNetworkIdentity;
    }

    public void SendNetworkMessage(Message message)
    {
        this.gameServer?.SendMessageToAll(message);
    }

    private void StartGameServer(GameServer gameServer)
    {
        this.gameServer = gameServer;

        // Registering Custom Messages
        this.gameServer.RegisterMessage<NetworkIdentityUpdate>();
        this.gameServer.RegisterMessage<NetworkIdentitiesDestroyed>();
        this.gameServer.RegisterMessage<NetworkBehaviourMessage>();
        this.gameServer.RegisterMessage<NetworkIdentityCreated>();

        // Messages
        this.gameServer.ServerReceivedMessage += this.ServerReceivedMessage;

        // Startup/Shutdown
        this.gameServer.ServerStarted += this.ServerStarted;
        this.gameServer.ServerShutdown += this.ServerShutdown;

        // User Connected / Disconnected
        this.gameServer.ServerUserConnected += this.ServerUserConnected;
        this.gameServer.ServerUserInfoUpdated += this.ServerUserInfoUpdated;
        this.gameServer.ServerUserDisconnected += this.ServerUserDisconnected;

        // Collecting all the static network objects
        NetworkManager.GetAllNetworkIdentities(this.staticNetworkObjectsList);

        // Hashing all the static network objects for quicker lookup
        foreach (var identity in this.staticNetworkObjectsList)
        {
            this.staticNetworkObjectsHash.Add(identity.NetworkId, identity);
            identity.Destroyed += this.NetworkIdentityDestroyed;
        }
    }

    private void NetworkIdentityDestroyed(long networkId)
    {
        if (this.staticNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity staticNetworkIdentity))
        {
            staticNetworkIdentity.Destroyed = null;

            this.staticNetworkObjectsHash.Remove(networkId);
            this.staticNetworkObjectsList.Remove(staticNetworkIdentity);
            this.destroyedStaticNetworkObjects.Add(networkId);

            // Telling all the clients it was destroyed
            networkIdentitiesDestroyedCache.DestroyedNetworkIds.Clear();
            networkIdentitiesDestroyedCache.DestroyedNetworkIds.Add(networkId);
            this.gameServer.SendMessageToAll(networkIdentitiesDestroyedCache);
        }
        else if (this.dynamicNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity dynamicNetworkIdentity))
        {
            dynamicNetworkIdentity.Destroyed = null;

            this.dynamicNetworkObjectsHash.Remove(networkId);
            this.dynamicNetworkObjectsList.Remove(dynamicNetworkIdentity);

            // Telling all the clients it was destroyed
            networkIdentitiesDestroyedCache.DestroyedNetworkIds.Clear();
            networkIdentitiesDestroyedCache.DestroyedNetworkIds.Add(networkId);
            this.gameServer.SendMessageToAll(networkIdentitiesDestroyedCache);
        }
        else
        {
            Debug.LogError("WTF");
        }
    }
    */
}
