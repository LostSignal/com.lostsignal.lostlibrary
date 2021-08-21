#pragma warning disable

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

    public interface IUpdatable
    {
        void DoUpdate(float deltaTime);
    }

    [Serializable]
    public class UpdateChannel
    {
        private struct Callback
        {
            public int Id;
            public IUpdatable Updatable;
            public float LastCalledTime;
            public UnityEngine.Object Context;
        }

        public enum UpdateType
        {
            Update,
            FixedUpdate,
            LateUpdate,
        }

        private static readonly Callback DefaultCallback = default(Callback);

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
            if (this.callbacks == null || this.callbacks.Length != startingCapacity)
            {
                this.callbacks = new Callback[startingCapacity];
            }
        }

        #if ENABLE_PROFILING
        public void InitializeSampler()
        {
            this.customSampler = CustomSampler.Create(this.name);
        }

        #endif

        private float runAllEveryXSeconds;
        private int nextIndexToRun;
        private int currentRunCount;
        private float currentDeltaTime = float.MaxValue;

        public void Run(float deltaTime)
        {
            if (this.count == 0)
            {
                return;
            }
            else if (currentDeltaTime > runAllEveryXSeconds)
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

            currentDeltaTime += deltaTime;

            // NOTE [bgish]: May need to make sure currentDeltaTime / runAllEveryXSeconds is never more than 1
            int desiredRunCount = Mathf.CeilToInt(currentDeltaTime / runAllEveryXSeconds) * this.count;
            int runCount = desiredRunCount - currentRunCount;

            if (runCount == 0) return;

            this.Run(runCount);

            //// Go backwards through the list starting form where we last left off and call the Actions
        }

        private void Reset()
        {
            nextIndexToRun = this.count - 1;
            currentRunCount = 0;
            currentDeltaTime = 0.0f;
        }

        private void Run(int runCount)
        {
            while (runCount > 0 && nextIndexToRun >= 0)
            {
                try
                {
                    float now = Time.realtimeSinceStartup;
                    float deltaTime = now - this.callbacks[nextIndexToRun].LastCalledTime;
                    this.callbacks[nextIndexToRun].LastCalledTime = now;
                    this.callbacks[nextIndexToRun].Updatable?.DoUpdate(deltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UpdateManager Channel {this.Name} caught an exception.", this.callbacks[nextIndexToRun].Context);
                    Debug.LogException(ex, this.callbacks[nextIndexToRun].Context);
                }

                this.nextIndexToRun--;
                this.currentRunCount++;
                runCount--;
            }
        }

        public UpdateChannelReceipt RegisterCallback(IUpdatable updatable, UnityEngine.Object context)
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

            idToIndexMap.Add(callbackId, callbackIndex);

            return UpdateChannelReceipt.New(callbackId, this.RemoveCallback);
        }

        private void RemoveCallback(int id)
        {
            if (idToIndexMap.TryGetValue(id, out int index))
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
    }
}

#endif
