//-----------------------------------------------------------------------
// <copyright file="NetworkBehaviour.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2018_3_OR_NEWER

namespace Lost.Networking
{
    using UnityEngine;

    public enum NetworkUpdateType
    {
        Tick,
        Manual
    }

    public abstract class NetworkBehaviour : MonoBehaviour
    {
        private static NetworkBehaviourDataMessage behaviourDataMessage = new NetworkBehaviourDataMessage();
        private static NetworkBehaviourMessage behaviourMessage = new NetworkBehaviourMessage();
        private static NetworkWriter writer = new NetworkWriter();

#pragma warning disable 0649
        [HideInInspector, SerializeField] private NetworkIdentity networkIdentity;
        [SerializeField] private NetworkUpdateType networkUpdateType = NetworkUpdateType.Tick;
        [SerializeField] private float updateFrequency = 0.1f;
        [SerializeField] private bool sendReliable = true;
#pragma warning restore 0649

        private float lastSent;

        public NetworkIdentity Identity => this.networkIdentity;

        public long NetworkId => this.networkIdentity.NetworkId;

        public bool IsOwner => this.networkIdentity.IsOwner;

        public int BehaviourIndex => this.networkIdentity.GetBehaviourIndex(this);

        protected float UpdateFrequency => this.updateFrequency;

        public abstract void Serialize(NetworkWriter writer);

        public abstract void Deserialize(NetworkReader reader);

        public void SendNetworkData(string key, string value, BehaviourDataSendType sendType = BehaviourDataSendType.All)
        {
            behaviourDataMessage.NetworkId = this.NetworkId;
            behaviourDataMessage.BehaviourIndex = this.BehaviourIndex;
            behaviourDataMessage.SendType = sendType;
            behaviourDataMessage.DataKey = key;
            behaviourDataMessage.DataValue = value;

            this.Identity.SendMessage(behaviourDataMessage, this.sendReliable);
        }

        public void SendNetworkBehaviourMessage(bool forceReliable = false)
        {
            if (this.IsOwner)
            {
                behaviourMessage.NetworkId = this.NetworkId;
                behaviourMessage.BehaviourIndex = this.BehaviourIndex;

                writer.SeekZero();
                this.Serialize(writer);

                behaviourMessage.DataLength = writer.Position;
                behaviourMessage.DataBytes = writer.RawBuffer;

                this.Identity.SendMessage(behaviourMessage, forceReliable ? true : this.sendReliable);
            }
        }

        public virtual void OnReceiveNetworkData(string dataType, string dataJson = null)
        {
        }

        protected virtual void NetworkUpdate()
        {
            if (this.IsOwner && ((Time.realtimeSinceStartup - this.lastSent) > this.updateFrequency))
            {
                this.SendNetworkBehaviourMessage();
                this.lastSent = Time.realtimeSinceStartup;
            }
        }

        protected virtual void OnValidate()
        {
            this.AssertGetComponentInParent(ref this.networkIdentity);
        }

        protected virtual void Awake()
        {
            this.AssertGetComponentInParent(ref this.networkIdentity);
            Debug.Assert(this.BehaviourIndex != -1);

            if (this.networkIdentity == null)
            {
                Debug.LogError($"NetworkBehaviour {this.name} couldn't find a NetworkIdentity.  It will not work properly!", this);
            }

            if (this.networkUpdateType == NetworkUpdateType.Tick)
            {
                this.Identity.NetworkUpdate += this.NetworkUpdate;
            }
        }
    }
}

#endif
