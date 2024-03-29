//-----------------------------------------------------------------------
// <copyright file="LoadBalancingManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    ////
    //// TODO [bgish]: Update QueueWork to also take in a IEnumerator and have it run Coroutines as well,
    ////               making our own Coroutine system is not trivial though, so not sure if that is the
    ////               best solution.
    ////
    public abstract class LoadBalancingManager<TManager, TInterface> : Manager<TManager>, IUpdate
        where TManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private int initialCapacity = 250;
        [SerializeField] private double maxMilliseconds = 0.1f;

        //// [SerializeField] private bool printDebugOutput = false;
#pragma warning restore 0649

        private Queue<Callback> callbackQueue;
        private UpdateChannelReceipt updateReceipt;
        private UpdateChannel updateChannel;

        // Stats
        private float maxQueuedTime = float.MinValue;
        private float totalQueuedTime = 0.0f;
        private int totalCallbacksProcessed = 0;
#pragma warning disable IDE0052  // Suppressing the warning, because eventurally we'll send this info via log and/or analytic
        private float averageQueuedTime = 0.0f;
#pragma warning restore IDE0052

        public abstract string Name { get; }

        public bool IsProcessing
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.callbackQueue.Count > 0;
        }

        public override void Initialize()
        {
            this.callbackQueue = new Queue<Callback>(this.initialCapacity);
            this.callbackQueue.OnGrow += this.OnQueueGrow;

            // AwakeManager is meant for gameplay elements, don't initialize till all managers are ready
            Bootloader.OnManagersReady += RegisterWithUpdateManager;

            void RegisterWithUpdateManager()
            {
                this.updateChannel = UpdateManager.Instance.GetChannel(this.Name);
                this.updateReceipt = this.updateChannel.RegisterCallback(this, this);
            }
        }

        public LoadBalancerReceipt QueueWork(TInterface action, string description, UnityEngine.Object context)
        {
            int index = this.callbackQueue.Enqueue(new Callback
            {
                Action = action,
                Description = description,
                Context = context,
                QueuedTime = Time.realtimeSinceStartup,
            });

            return LoadBalancerReceipt.New(index, context, this.CancelReceipt);
        }

        public virtual void OnUpdate(float deltaTime)
        {
            if (this.callbackQueue.IsEmpty)
            {
                return;
            }

            double startTime = Time.realtimeSinceStartupAsDouble;

            while (this.callbackQueue.Count > 0 && ((Time.realtimeSinceStartupAsDouble - startTime) * 1000.0) < this.maxMilliseconds)
            {
                Callback callback = this.callbackQueue.Dequeue();

                // Making sure this callback wasn't deleted
                if (callback.Action == null)
                {
                    continue;
                }

                try
                {
                    this.Execute(callback.Action);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{this.Name} Caught an Exception: {callback.Description}", callback.Context);
                    Debug.LogException(ex, callback.Context);
                }

                //// TODO [bgish]: Track the QueueTime and keep some stats on average/max time something sat in the queue
                //// TODO [bgish]: Maybe also track when a level activation has happened and the time it took to process that activation
                float totalQueuedTime = Time.realtimeSinceStartup - callback.QueuedTime;

                if (totalQueuedTime > this.maxQueuedTime)
                {
                    this.maxQueuedTime = totalQueuedTime;
                }

                this.totalQueuedTime += totalQueuedTime;
                this.totalCallbacksProcessed++;
                this.averageQueuedTime = this.totalQueuedTime / this.totalCallbacksProcessed;
            }
        }

        protected abstract void Execute(TInterface action);

        protected override void OnDisable()
        {
            base.OnDisable();
            this.updateReceipt.Cancel();
        }

        private void CancelReceipt(int index, UnityEngine.Object context)
        {
            var callback = this.callbackQueue.GetElementAt(index);

            if (callback.Context == context)
            {
                this.callbackQueue.DeleteElementAt(index);
            }
        }

        private void OnQueueGrow()
        {
            Debug.LogWarning($"{this.Name} Queue had to increase in size!  It's recommended to increase the initial capacity so this doesn't happen at runtime.");
        }

        private struct Callback
        {
            public TInterface Action;
            public float QueuedTime;
            public UnityEngine.Object Context;
            public string Description;
        }
    }
}

#endif
