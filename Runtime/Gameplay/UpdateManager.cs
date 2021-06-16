//-----------------------------------------------------------------------
// <copyright file="UpdateManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// Should Loadbalances themselves register with the update manager
// If that's the case, most classes just have loadbanalancer and don't care about the update manager
//    They register them, then the update manager can call things like stop LBs (durring load times)
// 
// Right now LBs assume that you Invoke and remove, this doesn't make sense for long running things
// 
// Can I use the trick of Replacing the last item with the currently deleted one?
// 
//
// Need a Delay Execute function as well
//
// What's the best way to handle Creating Channels
// What's the best way to handle adding/removing udpate functions
//
// How will LostActions work?  They go over multiple frames but must be called every frame
//   * Need load balancer that must go through all items?
//
// Manager Queue up Callbacks then pass them to LoadBalancerManager when ready?

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    [DefaultExecutionOrder(-1000)]
    public sealed class UpdateManager : Manager<UpdateManager>
    {
        private static List<FunctionCall> functions = new List<FunctionCall>();

        #pragma warning disable 0649
        [SerializeField] private List<UpdateChannel> channels = new List<UpdateChannel>();
        #pragma warning restore 0649

        private Dictionary<string, UpdateChannel> channelMap = new Dictionary<string, UpdateChannel>();

        public override void Initialize()
        {
            for (int i = 0; i < this.channels.Count; i++)
            {
                this.channelMap.Add(this.channels[i].Name, this.channels[i]);
            }

            Debug.Log("UpdateManager.Initialize");
            this.SetInstance(this);
        }

        public UpdateChannel GetOrCreateChannel(string channelName, int startingCapacity, float desiredDeltaTime = 0.0f)
        {
            if (this.channelMap.TryGetValue(channelName, out UpdateChannel channel))
            {
                return channel;
            }
            else
            {
                Debug.LogWarning($"Channel {channelName} created at runtime.  Should register this ahead of time to limit GC allocations.");
                var newChannel = new UpdateChannel(channelName, startingCapacity, desiredDeltaTime);
                this.channelMap.Add(newChannel.Name, newChannel);
                return newChannel;
            }
        }

        public void RegisterFunction(System.Action<float> function, int callFrequency)
        {
            UnregisterFunction(function);

            functions.Add(new FunctionCall
            {
                LastCallTime = Time.realtimeSinceStartup,
                Function = function,
                CallFrequency = callFrequency,
            });
        }

        public void UnregisterFunction(System.Action<float> function)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].Function == function)
                {
                    functions.RemoveAt(i);
                    break;
                }
            }
        }

        private void Update()
        {
            var realtimeSinceStartup = Time.realtimeSinceStartup;

            for (int i = 0; i < functions.Count; i++)
            {
                var functionCall = functions[i];
                var deltaTime = realtimeSinceStartup - functionCall.LastCallTime;

                if (deltaTime > (1.0f / functionCall.CallFrequency))
                {
                    functionCall.LastCallTime = realtimeSinceStartup;
                    functionCall.Function.Invoke(deltaTime);
                    functions[i] = functionCall;
                }
            }
        }

        public struct FunctionCall
        {
            public int CallFrequency;
            public float LastCallTime;
            public System.Action<float> Function;
        }

    }

    public struct CallbackReceipt
    {
        public static readonly CallbackReceipt Default = default(CallbackReceipt);

        public bool IsValid;
        public UpdateChannel Channel;
        public int Id;

        public void Cancel()
        {
            if (this.Channel != null)
            {
                this.Channel.RemoveCallback(ref this);
            }
        }
    }
    
    public struct Callback
    {
        public int Id;
        public Action<float> Action;
        public float LastCalledTime;
        public UnityEngine.Object Context;
    }

    [Serializable]
    public class UpdateChannel
    {
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
            // Go backwards through the list starting form where we last left off and call the Actions
    
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
    
        public void RegisterCallback(ref CallbackReceipt callbackReceipt, Action<float> action, UnityEngine.Object context)
        {
            if (callbackReceipt.Channel == this)
            {
                return;
            }
            else if (callbackReceipt.Channel != null)
            {
                callbackReceipt.Cancel();
            }

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
            callbackReceipt = new CallbackReceipt { Channel = this, Id = callbackId };
        }
    
        public void RemoveCallback(ref CallbackReceipt receipt)
        {
            if (idToIndexMap.TryGetValue(receipt.Id, out int index))
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

                receipt = CallbackReceipt.Default;
            }
        }
    }
    
    //// public class UpdateQueue
    //// {
    ////     private const int StartingCapacity = 100;
    ////     private List<CallBack> callbacks = new List<CallBack>(StartingCapacity);
    ////     private List<int> freeIndices = new List<int>(StartingCapacity);
    ////     private string name;    
    ////     private int count;
    //// 
    ////     public int Count => this.count;
    ////   
    ////     private CallbackQueue(string name)
    ////     {
    ////         this.name = name;
    ////   
    ////         for (int i = 0 ; i < StartingCapacity; i++)
    ////         {
    ////             freeIndices.Add(StartingCapacity - i);
    ////         }
    ////     }
    //// 
    ////     public CallbackReceipt Add(Action action, string description)
    ////     {
    ////     
    ////   
    ////     }
    //// 
    ////     public CallbackReceipt(Remove callbackRecipt)
    ////     {
    ////     }
    //// }
}
