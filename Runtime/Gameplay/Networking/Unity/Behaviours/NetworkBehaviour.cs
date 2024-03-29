//-----------------------------------------------------------------------
// <copyright file="NetworkBehaviour.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Networking
{
    using System;
    using UnityEngine;

    public enum NetworkUpdateType
    {
        Tick,
        Manual,
    }

    public abstract class NetworkBehaviour : MonoBehaviour
    {
        private static readonly NetworkBehaviourDataMessage BehaviourDataMessage = new NetworkBehaviourDataMessage();
        private static readonly NetworkBehaviourMessage BehaviourMessage = new NetworkBehaviourMessage();
        private static readonly NetworkWriter Writer = new NetworkWriter();

#pragma warning disable 0649
        [SerializeField] private NetworkIdentity networkIdentity;
#pragma warning restore 0649

        private SendConfig sendConfig;
        private float lastSent;

        public NetworkIdentity Identity => this.networkIdentity;

        public long NetworkId => this.networkIdentity.NetworkId;

        public bool IsOwner => this.networkIdentity.IsOwner;

        public int BehaviourIndex => this.networkIdentity.GetBehaviourIndex(this);

        protected float UpdateFrequency => this.sendConfig.UpdateFrequency;

        public abstract void Serialize(NetworkWriter writer);

        public abstract void Deserialize(NetworkReader reader);

        public void SendNetworkData(string key, string value, NetworkBehaviourDataSendType sendType = NetworkBehaviourDataSendType.All)
        {
            BehaviourDataMessage.NetworkId = this.NetworkId;
            BehaviourDataMessage.BehaviourIndex = this.BehaviourIndex;
            BehaviourDataMessage.SendType = sendType;
            BehaviourDataMessage.DataKey = key;
            BehaviourDataMessage.DataValue = value;

            this.Identity.SendNetworkMessage(BehaviourDataMessage, this.sendConfig.SendReliable);
        }

        public void SendNetworkBehaviourMessage(bool forceReliable = false)
        {
            if (this.IsOwner)
            {
                BehaviourMessage.NetworkId = this.NetworkId;
                BehaviourMessage.BehaviourIndex = this.BehaviourIndex;

                Writer.SeekZero();
                this.Serialize(Writer);

                BehaviourMessage.DataLength = Writer.Position;
                BehaviourMessage.DataBytes = Writer.RawBuffer;

                this.Identity.SendNetworkMessage(BehaviourMessage, forceReliable || this.sendConfig.SendReliable);
            }
        }

        public virtual void OnReceiveNetworkData(string dataType, string dataJson = null)
        {
        }

        protected abstract SendConfig GetInitialSendConfig();

        protected void UpdateSendConfig(SendConfig sendConfig)
        {
            this.sendConfig = sendConfig;

            this.Identity.NetworkUpdate -= this.NetworkUpdate;

            if (this.sendConfig.NetworkUpdateType == NetworkUpdateType.Tick)
            {
                this.Identity.NetworkUpdate += this.NetworkUpdate;
            }
        }

        protected virtual void NetworkUpdate()
        {
            if (this.IsOwner && ((Time.realtimeSinceStartup - this.lastSent) > this.sendConfig.UpdateFrequency))
            {
                this.SendNetworkBehaviourMessage();
                this.lastSent = Time.realtimeSinceStartup;
            }
        }

        protected virtual void OnValidate()
        {
            // Making sure we have a NetworkIdentity and we belong to it (editor only)
            if (Application.isEditor && Application.isPlaying == false && this.networkIdentity == null)
            {
                var networkIdentity = this.GetComponentInParent<NetworkIdentity>();

                if (networkIdentity == null)
                {
                    networkIdentity = this.GetOrAddComponent<NetworkIdentity>();
                }

                networkIdentity.EditorOnlyAddBehaviour(this);
            }

            this.AssertGetComponentInParent(ref this.networkIdentity);
        }

        protected virtual void Awake()
        {
            this.OnValidate();

            Debug.Assert(this.BehaviourIndex != -1);

            if (this.networkIdentity == null)
            {
                Debug.LogError($"NetworkBehaviour {this.name} couldn't find a NetworkIdentity.  It will not work properly!", this);
            }

            this.UpdateSendConfig(this.GetInitialSendConfig());
        }

        [Serializable]
        public struct SendConfig
        {
            public NetworkUpdateType NetworkUpdateType;
            public float UpdateFrequency;
            public bool SendReliable;
        }
    }
}

#endif
