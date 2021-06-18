//-----------------------------------------------------------------------
// <copyright file="UpdateChannel.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class UpdateChannel
    {
        private struct Callback
        {
            public int Id;
            public Action<float> Action;
            public float LastCalledTime;
            public UnityEngine.Object Context;
        }

        private static readonly Callback DefaultCallback = default(Callback);

        #pragma warning disable 0649
        [SerializeField] private string name;
        [SerializeField] private int startingCapacity = 10;
        #pragma warning restore 0649

        private Dictionary<int, int> idToIndexMap = new Dictionary<int, int>();
        private Callback[] callbacks;
        private int currentId;
        private int count;

        public string Name => this.name;
    
        public UpdateChannel(string name, int startingCapactiy, float desiredDeltaTime)
        {
            this.name = name;
            this.callbacks = new Callback[startingCapacity];
        }
    
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
                    this.callbacks[nextIndexToRun].Action?.Invoke(deltaTime);
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
    
        public UpdateChannelReceipt RegisterCallback(Action<float> action, UnityEngine.Object context)
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
                Action = action,
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
