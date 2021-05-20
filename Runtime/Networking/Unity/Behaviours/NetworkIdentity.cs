//-----------------------------------------------------------------------
// <copyright file="NetworkIdentity.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public delegate void NetworkUpdateDelegate();
    public delegate void NetworkOwnershipRequestedDelegate();
    public delegate void NetworkOwnershipGrantedDelegate();
    public delegate void NetworkOwnershipFailedDelegate();

    public class NetworkIdentity : MonoBehaviour
    {
        private static readonly NetworkIdentityRequestUpdate NetworkIdentityRequestUpdateCache = new NetworkIdentityRequestUpdate();
        private static readonly NetworkIdentityOwnershipRequest NetworkIdentityOwnershipRequestCache = new NetworkIdentityOwnershipRequest();
        private static readonly NetworkIdentityReleaseOwnershipRequest NetworkIdentityReleaseOwnershipRequestCache = new NetworkIdentityReleaseOwnershipRequest();


        public event NetworkUpdateDelegate NetworkUpdate;

        public static long NewId()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }

        private static readonly NetworkIdentityUpdate UpdateNetworkIdentityMessageCache = new NetworkIdentityUpdate();
        private static readonly List<NetworkIdentity> FoundNetworkIdentitiesCache = new List<NetworkIdentity>();

        public delegate void NetworkIdentityDestroyedDelegate(long networkId);

        public NetworkIdentityDestroyedDelegate Destroyed;
        public NetworkOwnershipRequestedDelegate OwnershipRequested;
        public NetworkOwnershipGrantedDelegate OwnershipRequestGranted;
        public NetworkOwnershipFailedDelegate OwnershipRequestFailed;

#pragma warning disable 0649
        [SerializeField] private long networkId = -1L;
        [SerializeField] private NetworkBehaviour[] behaviours = new NetworkBehaviour[0];
        [SerializeField] private bool destoryOnDisconnect;
        [SerializeField] private bool canChangeOwner;
#pragma warning restore 0649

        private GameClient gameClient = null;
        private bool isRequestingOwnership;
        private long ownerId;

        public long NetworkId => this.networkId;

        public long OwnerId => this.ownerId;

        public string ResourceName { get; set; }

        public bool IsOwner => this.gameClient?.IsConnected == true && this.ownerId == this.gameClient.UserId;

        public bool IsRequestingOwnership => this.isRequestingOwnership;

        public bool DestoryOnDisconnect => this.destoryOnDisconnect;

        public bool CanChangeOwner => this.canChangeOwner;

        public NetworkBehaviour[] Behaviours => this.behaviours;

        public static void GetAllSceneNetworkIdentities(List<NetworkIdentity> networkIdentities)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                GetNetworkIdentitiesFromScene(networkIdentities, SceneManager.GetSceneAt(i));
            }
        }

        public static void GetNetworkIdentitiesFromScene(List<NetworkIdentity> networkIdentities, Scene scene)
        {
            if (scene.isLoaded)
            {
                foreach (var rootObject in scene.GetRootGameObjects())
                {
                    FoundNetworkIdentitiesCache.Clear();
                    rootObject.GetComponentsInChildren<NetworkIdentity>(true, FoundNetworkIdentitiesCache);
                    networkIdentities.AddRange(FoundNetworkIdentitiesCache);
                }
            }
        }

        public void EditorOnlyAddBehaviour(NetworkBehaviour networkBehaviour)
        {
            if (Application.isEditor && Application.isPlaying == false)
            {
                List<NetworkBehaviour> allBehaviours = new List<NetworkBehaviour>(this.behaviours);
                if (allBehaviours.Contains(networkBehaviour) == false)
                {
                    allBehaviours.Add(networkBehaviour);
                    this.behaviours = allBehaviours.ToArray();
                }
            }
            else
            {
                Debug.LogError("EditorOnlyAddBehaviour was called when not in editor.");
            }
        }

        public void RequestOwnership()
        {
            if (this.canChangeOwner)
            {
                this.isRequestingOwnership = true;
                this.OwnershipRequested?.Invoke();

                NetworkIdentityOwnershipRequestCache.NetworkId = this.networkId;
                this.gameClient?.SendMessage(NetworkIdentityOwnershipRequestCache);

                if (NetworkingManager.PrintDebugOutput)
                {
                    Debug.Log($"Requesting Ownership of NetworkIdentity {this.networkId}");
                }
            }
        }

        public void ReleaseOwnership()
        {
            if (this.IsOwner)
            {
                NetworkIdentityReleaseOwnershipRequestCache.NetworkId = this.networkId;
                this.gameClient?.SendMessage(NetworkIdentityReleaseOwnershipRequestCache);

                if (NetworkingManager.PrintDebugOutput)
                {
                    Debug.Log($"Releasing Ownership of NetworkIdentity {this.networkId}");
                }
            }
        }

        public void SetNetworkId(long networkId)
        {
            this.networkId = networkId;
        }

        public void GenerateNewNetworkId()
        {
            this.networkId = NewId();
        }

        public void SetOwner(long ownerId, bool canChangeOwner)
        {
            if (this.isRequestingOwnership)
            {
                if (ownerId == this.gameClient?.UserId)
                {
                    if (NetworkingManager.PrintDebugOutput)
                    {
                        Debug.Log($"Ownership Granted for NetworkIdentity {this.networkId}");
                    }

                    this.OwnershipRequestGranted?.Invoke();
                }
                else
                {
                    if (NetworkingManager.PrintDebugOutput)
                    {
                        Debug.Log($"Ownership Failed for NetworkIdentity {this.networkId}");
                    }

                    this.OwnershipRequestFailed?.Invoke();
                }

                this.isRequestingOwnership = false;
            }

            this.ownerId = ownerId;
            this.canChangeOwner = canChangeOwner;
        }

        public void SetClient(GameClient gameClient)
        {
            this.gameClient = gameClient;
        }

        public void SendMessage(Message message, bool reliable = true)
        {
            this.gameClient.SendMessage(message, reliable);
        }

        public void RequestUpdate()
        {
            if (NetworkingManager.PrintDebugOutput)
            {
                Debug.Log($"NetworkIdentity.RequestUpdate {this.NetworkId}: {this.name}");
            }

            NetworkIdentityRequestUpdateCache.NetworkId = this.networkId;
            NetworkIdentityRequestUpdateCache.IsEnabled = this.gameObject.activeSelf;
            NetworkIdentityRequestUpdateCache.ResourceName = this.ResourceName;
            NetworkIdentityRequestUpdateCache.Position = this.transform.position;
            NetworkIdentityRequestUpdateCache.Rotation = this.transform.rotation;
            NetworkIdentityRequestUpdateCache.BehaviourCount = this.behaviours.Length;
            NetworkIdentityRequestUpdateCache.DestoryOnDisconnect = this.destoryOnDisconnect;
            NetworkIdentityRequestUpdateCache.CanChangeOwner = this.canChangeOwner;

            this.SendMessage(NetworkIdentityRequestUpdateCache);
        }

        public int GetBehaviourIndex(NetworkBehaviour behaviour)
        {
            if (this.behaviours != null)
            {
                for (int i = 0; i < this.behaviours.Length; i++)
                {
                    if (this.behaviours[i] == behaviour)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void OnValidate()
        {
            if (Application.isPlaying || Platform.IsApplicationQuitting)
            {
                return;
            }

            if (this.networkId == -1L)
            {
                this.networkId = NewId();
            }
            else
            {
                List<NetworkIdentity> networkIdentities = new List<NetworkIdentity>();
                GetAllSceneNetworkIdentities(networkIdentities);

                foreach (var networkIdentity in networkIdentities)
                {
                    if (networkIdentity != this && networkIdentity.networkId == this.networkId)
                    {
                        this.networkId = NewId();
                        break;
                    }
                }
            }
        }

        private void OnEnable()
        {
            this.SendUpdateNetworkIdentityMessage();
        }

        private void OnDisable()
        {
            this.SendUpdateNetworkIdentityMessage();

            // Making sure we send all our info on disable
            for (int i = 0; i < this.behaviours.Length; i++)
            {
                if (this.behaviours[i])
                {
                    this.behaviours[i].SendNetworkBehaviourMessage(true);
                }
            }
        }

        private void OnDestroy()
        {
            this.Destroyed?.Invoke(this.networkId);
        }

        private void Update()
        {
            this.NetworkUpdate?.Invoke();
        }

        private void SendUpdateNetworkIdentityMessage()
        {
            // if (NetworkingManager.IsInitialized && this.IsOwner)
            // {
            //     UpdateNetworkIdentityMessageCache.PopulateMessage(this);
            //     NetworkingManager.Instance.SendMessage(UpdateNetworkIdentityMessageCache);
            // }
        }
    }
}

#endif
