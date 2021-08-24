//-----------------------------------------------------------------------
// <copyright file="DelayExecuteManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using UnityEngine;

    public class DelayExecuteManager : Manager<DelayExecuteManager>, IUpdate
    {
        private const string ChannelName = nameof(DelayExecuteManager);

#pragma warning disable 0649
        [SerializeField] private int initialCapacity = 50;
#pragma warning restore 0649

        private UpdateChannelReceipt updateReceipt;
        private DelayedAction[] delayedActions;
        private int count;

        public override void Initialize()
        {
            Bootloader.OnManagersReady += SetupUpdateChannel;

            this.delayedActions = new DelayedAction[this.initialCapacity];
            this.count = 0;

            this.SetInstance(this);

            void SetupUpdateChannel()
            {
                var updateChannel = UpdateManager.Instance.GetChannel(ChannelName);

                if (updateChannel == null)
                {
                    Debug.LogError($"{nameof(DelayExecuteManager)} couldn't find Update Channel \"{ChannelName}\".  This manager will not work!", this);
                }
                else
                {
                    this.updateReceipt = updateChannel.RegisterCallback(this, this);
                }
            }
        }

        public void Add(Action action, float seconds)
        {
            if (this.count >= this.delayedActions.Length)
            {
                Debug.LogWarning("DelayExecuteManager had to grow in size at runtime.  Please update initialCapacity to stop this from happening.", this);
                Array.Resize(ref this.delayedActions, this.delayedActions.Length * 2);
            }

            this.delayedActions[this.count++] = new DelayedAction { ExecuteTime = Time.realtimeSinceStartup + seconds, Action = action };
        }

        void IUpdate.OnUpdate(float deltaTime)
        {
            float currentTime = Time.realtimeSinceStartup;
            int i = 0;

            while (i < this.count)
            {
                if (this.delayedActions[i].ExecuteTime <= currentTime)
                {
                    this.delayedActions[i].Action?.Invoke();

                    int lastIndex = this.count - 1;

                    if (i != lastIndex)
                    {
                        this.delayedActions[i] = this.delayedActions[lastIndex];
                    }

                    this.delayedActions[lastIndex] = default;

                    currentTime = Time.realtimeSinceStartup;
                    this.count--;
                }
                else
                {
                    i++;
                }
            }
        }

        private void OnDestroy()
        {
            this.updateReceipt.Cancel();
        }

        public struct DelayedAction
        {
            public float ExecuteTime;
            public Action Action;
        }
    }
}

#endif
