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
        public struct FunctionCall
        {
            public int CallFrequency;
            public float LastCallTime;
            public Action<float> Function;
        }

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

        public UpdateChannel GetChannel(string channelName)
        {
            if (this.channelMap.TryGetValue(channelName, out UpdateChannel channel))
            {
                return channel;
            }
            
            Debug.LogError($"Unknown Channel {channelName} Requested!  Channels can not be created at runtime, you must add them to the UpdateManager ahead of time.");
            return null;
        }

        //// TODO [bgish]: Only PeriodicUpdate Bolt Node uses this, update it to use Channel system
        public void RegisterFunction(Action<float> function, int callFrequency)
        {
            UnregisterFunction(function);

            functions.Add(new FunctionCall
            {
                LastCallTime = Time.realtimeSinceStartup,
                Function = function,
                CallFrequency = callFrequency,
            });
        }
        
        //// TODO [bgish]: Only PeriodicUpdate Bolt Node uses this, update it to use Channel system
        public void UnregisterFunction(Action<float> function)
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
    }
}
