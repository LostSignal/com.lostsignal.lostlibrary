//-----------------------------------------------------------------------
// <copyright file="UpdateChannel.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

#define ENABLE_PROFILING

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Profiling;

    public interface IUpdate
    {
        void OnUpdate(float deltaTime);
    }

    [Serializable]
    public class UpdateChannel
    {
        private static readonly Callback DefaultCallback = default;

        #pragma warning disable 0649
        [SerializeField] private string name;
        [SerializeField] private float desiredDeltaTime = 0.0f;
        [SerializeField] private UpdateType updateType;
        [SerializeField] private int startingCapacity = 10;
        #pragma warning restore 0649

        #if ENABLE_PROFILING
        private CustomSampler customSampler;
        #endif

        private Dictionary<int, int> idToIndexMap = new Dictionary<int, int>();
        private Callback[] callbacks;
        private int currentId;
        private int count;

        private float runAllEveryXSeconds;
        private int nextIndexToRun;
        private int currentRunCount;
        private float currentDeltaTime = float.MaxValue;

        public enum UpdateType
        {
            Update,
            FixedUpdate,
            LateUpdate,
        }

        public string Name => this.name;

        public UpdateType Type => this.updateType;

        public float DesiredDeltaTime
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.desiredDeltaTime;
        }

        public CustomSampler CustomSampler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.customSampler;
        }

        public void Initialize()
        {
            if (this.callbacks == null || this.callbacks.Length != this.startingCapacity)
            {
                this.callbacks = new Callback[this.startingCapacity];
            }
        }

        #if ENABLE_PROFILING
        public void InitializeSampler()
        {
            this.customSampler = CustomSampler.Create(this.name);
        }

        #endif


        public void Run(float deltaTime)
        {
            if (this.count == 0)
            {
                return;
            }
            else if (this.currentDeltaTime > this.runAllEveryXSeconds)
            {
                int runsRemaining = this.nextIndexToRun + 1;

                if (runsRemaining != 0)
                {
                    this.Run(runsRemaining);
                    this.Reset();
                    return;
                }
                else
                {
                    this.Reset();
                }
            }

            this.currentDeltaTime += deltaTime;

            // NOTE [bgish]: May need to make sure currentDeltaTime / runAllEveryXSeconds is never more than 1
            int desiredRunCount = Mathf.CeilToInt(this.currentDeltaTime / this.runAllEveryXSeconds) * this.count;
            int runCount = desiredRunCount - this.currentRunCount;

            if (runCount == 0)
            {
                return;
            }

            this.Run(runCount);

            //// Go backwards through the list starting form where we last left off and call the Actions
        }

        public UpdateChannelReceipt RegisterCallback(IUpdate updatable, UnityEngine.Object context)
        {
            int callbackIndex = this.count++;

            if (callbackIndex >= this.callbacks.Length)
            {
                Debug.LogError($"Channel {this.name} had to increase it's size.  You should register the channel with the UpdateManager and increase it's initial capacity.");
                Array.Resize(ref this.callbacks, this.callbacks.Length * 2);
            }

            int callbackId = ++this.currentId;

            this.callbacks[callbackIndex] = new Callback
            {
                Id = callbackId,
                Updatable = updatable,
                Context = context,
                LastCalledTime = Time.realtimeSinceStartup,
            };

            this.idToIndexMap.Add(callbackId, callbackIndex);

            return UpdateChannelReceipt.New(callbackId, this.RemoveCallback);
        }

        private void Reset()
        {
            this.nextIndexToRun = this.count - 1;
            this.currentRunCount = 0;
            this.currentDeltaTime = 0.0f;
        }

        private void Run(int runCount)
        {
            while (runCount > 0 && this.nextIndexToRun >= 0)
            {
                try
                {
                    float now = Time.realtimeSinceStartup;
                    float deltaTime = now - this.callbacks[this.nextIndexToRun].LastCalledTime;
                    this.callbacks[this.nextIndexToRun].LastCalledTime = now;
                    this.callbacks[this.nextIndexToRun].Updatable?.OnUpdate(deltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UpdateManager Channel {this.Name} caught an exception.", this.callbacks[this.nextIndexToRun].Context);
                    Debug.LogException(ex, this.callbacks[this.nextIndexToRun].Context);
                }

                this.nextIndexToRun--;
                this.currentRunCount++;
                runCount--;
            }
        }

        private void RemoveCallback(int id)
        {
            if (this.idToIndexMap.TryGetValue(id, out int index))
            {
                var lastCallbackIndex = this.count - 1;
                var callbackToRemove = this.callbacks[index];

                if (index != lastCallbackIndex)
                {
                    var lastCallback = this.callbacks[lastCallbackIndex];

                    this.callbacks[index] = lastCallback;
                    this.idToIndexMap[lastCallback.Id] = index;
                }

                this.idToIndexMap.Remove(callbackToRemove.Id);
                this.callbacks[lastCallbackIndex] = DefaultCallback;
                this.count--;
            }
        }

        private struct Callback
        {
            public int Id;
            public IUpdate Updatable;
            public float LastCalledTime;
            public UnityEngine.Object Context;
        }
    }
}

#endif
