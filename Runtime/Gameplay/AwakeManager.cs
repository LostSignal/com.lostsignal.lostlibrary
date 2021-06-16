//-----------------------------------------------------------------------
// <copyright file="AwakeManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
    public class AwakeManager : Manager<AwakeManager>
    {
        private struct Callback
        {
            public Action Action;
        
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            public float QueuedTime;
            public UnityEngine.Object Context;
            public string Description;
            #endif

            public void Invoke()
            {
                try
                {
                    this.Action?.Invoke();
                }
                catch (Exception ex)
                {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogError($"AwakeManager Caught an Exception: {this.Description}", this.Context);
                    Debug.LogException(ex, this.Context);
                    #else
                    Debug.LogError("AwakeManager Caught an Exception");
                    Debug.LogException(ex);
                    #endif
                }
            }
        }

        #pragma warning disable 0649
        [SerializeField] private int initialCapacity = 250;
        [SerializeField] private double maxMilliseconds = 0.1f;
        //[SerializeField] private bool printDebugOutput = false;
        #pragma warning restore 0649
        
        private Queue<Callback> callbackQueue;
        private Channel awakeManagerChannel;

        // Stats
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private float maxQueuedTime = float.MinValue;
        private float totalQueuedTime = 0.0f;
        private int totalCallbacksProcessed = 0;
        #pragma warning disable IDE0052  // Suppressing the warning, because eventurally we'll send this info via log and/or analytic
        private float averageQueuedTime = 0.0f;
        #pragma warning restore IDE0052
        #endif

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

            Debug.Log("AwakeManager.Initialize");
            this.SetInstance(this);

            void RegisterWithUpdateManager()
            {
                this.awakeManagerChannel = UpdateManager.Instance.GetOrCreateChannel("AwakeManager", 1, 1000);
                this.awakeManagerChannel.AddCallback(this.DoWork, "AwakeManager", this);
            }
        }

        public void QueueWork(Action action, string description, UnityEngine.Object context)
        {
            this.callbackQueue.Enqueue(new Callback
            {
                Action = action,

                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Description = description,
                Context = context,
                QueuedTime = Time.realtimeSinceStartup,
                #endif
            });
        }

        private void DoWork(float deltaTime)
        {
            if (this.callbackQueue.IsEmpty)
            {
                return;
            }

            DateTime startTime = DateTime.Now;

            while (this.callbackQueue.Count > 0 && DateTime.Now.Subtract(startTime).TotalMilliseconds < this.maxMilliseconds)
            {
                Callback callback = this.callbackQueue.Dequeue();
                callback.Invoke();

                //// TODO [bgish]: Track the QueueTime and keep some stats on average/max time something sat in the queue
                //// TODO [bgish]: Maybe also track when a level activation has happened and the time it took to process that activation
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                float totalQueuedTime = Time.realtimeSinceStartup - callback.QueuedTime;

                if (totalQueuedTime > this.maxQueuedTime)
                {
                    this.maxQueuedTime = totalQueuedTime;
                }

                this.totalQueuedTime += totalQueuedTime;
                this.totalCallbacksProcessed++;
                this.averageQueuedTime = this.totalQueuedTime / this.totalCallbacksProcessed;
                #endif
            }
        }

        private void OnQueueGrow()
        {
            Debug.LogWarning("AwakeManager Queue had to increase in size!  It's recommended to increase the initial capacity so this doesn't happen at runtime.");
        }
    }
}
