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


        public override void Initialize()
        {
            this.SetInstance(this);
        }

        
        public void CreateChannel(string channelName, int callsPerSecond)
        {
            // If it makes one at runtime, print warning (channel created at runtime, make ahead of time to reduce GC)
            throw new System.NotImplementedException();
        }

        public void Register(string channelName, System.Action<float> function)
        {
            throw new System.NotImplementedException();
        }


        public class Channel
        {
            public string Name;
            public int CallFrequency;
            public List<FunctionCall> Functions;
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
        public Channel Channel;
        public int Id;
    }
    
    public struct Callback
    {
        public int Id;
        public Action Action;
        public float LastCalledTime;
        public string Description;         // Editor Only
        public UnityEngine.Object Context; // Editor Only
    }

    [Serializable]
    public class Channel
    {
        #pragma warning disable 0649
        [SerializeField] private int startingCapacity = 10;
        #pragma warning restore 0649

        private Dictionary<int, int> idToIndexMap = new Dictionary<int, int>();
        private List<Callback> callbacks;
        private int currentId;
    
        public Channel(int startingCapactiy, float runAllEveryXSeconds)
        { 
            callbacks = new List<Callback>(startingCapacity);
        }
    
        private float runAllEveryXSeconds;
        private int nextIndexToRun;
        private int currentRunCount;
        private float currentDeltaTime = float.MaxValue;
    
        public void Run(float deltaTime)
        {
            if (this.callbacks.Count == 0)
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
            int desiredRunCount = Mathf.CeilToInt(currentDeltaTime / runAllEveryXSeconds) * this.callbacks.Count;
            int runCount = desiredRunCount - currentRunCount;
      
            if (runCount == 0) return;
      
            this.Run(runCount);
            // Go backwards through the list starting form where we last left off and call the Actions
    
        }
    
        private void Reset()
        {
             nextIndexToRun = this.callbacks.Count - 1; 
             currentRunCount = 0;
             currentDeltaTime = 0.0f;
        }
    
        private void Run(int runCount)
        {
            while (runCount > 0 && nextIndexToRun >= 0)
            {  
                try
                {
                    this.callbacks[nextIndexToRun].Action?.Invoke();
                }
                catch (Exception ex)
                {
                    // LOG AND CONTINUE
                }
                
                this.nextIndexToRun--;
                this.currentRunCount++;
                runCount--;
            }
        }
    
        public CallbackReceipt AddCallback(Action action, string description, UnityEngine.Object context)
        {
            int callbackIndex = this.callbacks.Count;
            int callbackId = ++this.currentId;
        
            this.callbacks.Add(new Callback
            {
              Id = callbackId,
              Action = action,
              Description = description,
              Context = context,
            });
      
            idToIndexMap.Add(callbackId, callbackIndex);
            return new CallbackReceipt { Channel = this, Id = callbackId };
        }
    
        public void RemoveCallback(CallbackReceipt receipt)
        {
            if (idToIndexMap.TryGetValue(receipt.Id, out int index))
            {
                 var lastCallbackIndex = this.callbacks.Count - 1;
                 var callbackToRemove = this.callbacks[index];
          
                 if (index != lastCallbackIndex)
                 {
                   var lastCallback = this.callbacks[lastCallbackIndex];

                   this.callbacks[index] = lastCallback;
                   this.idToIndexMap[lastCallback.Id] = index;
                 }
          
                 this.idToIndexMap.Remove(callbackToRemove.Id);
                 this.callbacks.RemoveAt(lastCallbackIndex);
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
