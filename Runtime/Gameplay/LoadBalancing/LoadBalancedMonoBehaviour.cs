//-----------------------------------------------------------------------
// <copyright file="LoadBalancedMonoBehaviour.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public abstract class LoadBalancedMonoBehaviour : MonoBehaviour, IUpdatable, IAwakable, IStartable
    {
        private LoadBalancerReceipt awakeReceipt;
        private LoadBalancerReceipt startReceipt;
        private UpdateChannelReceipt updateReceipt;
        private string currentUpdateChannelName;

        public abstract string Name { get; }

        public abstract bool RunAwake { get; }

        public abstract bool RunStart { get; }

        public abstract void DoUpdate(float deltaTime);

        protected virtual void Awake()
        {
            if (this.RunAwake)
            {
                if (Bootloader.AreManagersReady == false)
                {
                    Bootloader.OnManagersReady += OnManagersReady;
                }
                else
                {
                    this.awakeReceipt = AwakeManager.Instance.QueueWork(this, this.Name, this);
                }
            }

            void OnManagersReady()
            {
                this.awakeReceipt = AwakeManager.Instance.QueueWork(this, this.Name, this);
            }
        }

        public void DoAwake()
        {
            this.LoadBalancedAwake();
            this.awakeReceipt = default(LoadBalancerReceipt);
        }

        protected virtual void Start()
        {
            if (this.RunStart)
            {
                if (Bootloader.AreManagersReady == false)
                {
                    Bootloader.OnManagersReady += OnManagersReady;
                }
                else
                {
                    this.startReceipt = StartManager.Instance.QueueWork(this, this.name, this);
                }
            }

            void OnManagersReady()
            {
                this.startReceipt = StartManager.Instance.QueueWork(this, this.name, this);
            }
        }

        public void DoStart()
        {
            this.LoadBalancedStart();
            this.startReceipt = default(LoadBalancerReceipt);
        }

        protected virtual void OnDestroy()
        {
            this.awakeReceipt.Cancel();
            this.startReceipt.Cancel();
            this.updateReceipt.Cancel();
        }

        protected void StartUpadating(string updateChannelName)
        {
            if (this.currentUpdateChannelName == updateChannelName)
            {
                return;
            }

            this.updateReceipt.Cancel();
            this.currentUpdateChannelName = updateChannelName;
            var updateChannel = UpdateManager.Instance.GetChannel(updateChannelName);
            this.updateReceipt = updateChannel.RegisterCallback(this, this);
        }

        protected void StopUpdating()
        {
            this.updateReceipt.Cancel();
            this.currentUpdateChannelName = null;
        }

        protected virtual void LoadBalancedAwake()
        {
        }

        protected virtual void LoadBalancedStart()
        {
        }
    }
}
