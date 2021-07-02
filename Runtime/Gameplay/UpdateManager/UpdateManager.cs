//-----------------------------------------------------------------------
// <copyright file="UpdateManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//// 
//// 
//// 
//// Should Loadbalances themselves register with the update manager
//// If that's the case, most classes just have loadbanalancer and don't care about the update manager
////    They register them, then the update manager can call things like stop LBs (durring load times)
//// 
//// Right now LBs assume that you Invoke and remove, this doesn't make sense for long running things
//// 
//// Can I use the trick of Replacing the last item with the currently deleted one?
//// 
////
//// Need a Delay Execute function as well
////
//// What's the best way to handle Creating Channels
//// What's the best way to handle adding/removing udpate functions
////
//// How will LostActions work?  They go over multiple frames but must be called every frame
////   * Need load balancer that must go through all items?
////
//// Manager Queue up Callbacks then pass them to LoadBalancerManager when ready?
//// 
//// * Can I make a whole new "Update Manager" window in Unity's profiler that lets me see all my channels and how much time they're taking?
//// 
//// 
//// 
//// 


#define ENABLE_PROFILING

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
        
        private List<UpdateChannel> updateChannels = new List<UpdateChannel>(50);
        private List<UpdateChannel> fixedUpdateChannels = new List<UpdateChannel>(50);
        private List<UpdateChannel> lateUpdateChannels = new List<UpdateChannel>(50);

        public override void Initialize()
        {
            for (int i = 0; i < this.channels.Count; i++)
            {
                this.channels[i].Initialize();

                #if ENABLE_PROFILING
                this.channels[i].InitializeSampler();
                #endif

                this.channelMap.Add(this.channels[i].Name, this.channels[i]);

                switch (this.channels[i].Type)
                {
                    case UpdateChannel.UpdateType.Update:
                        {
                            this.updateChannels.Add(this.channels[i]);
                            break;
                        }
                        
                    case UpdateChannel.UpdateType.FixedUpdate:
                        {
                            this.fixedUpdateChannels.Add(this.channels[i]);
                            break;
                        }

                    case UpdateChannel.UpdateType.LateUpdate:
                        {
                            this.lateUpdateChannels.Add(this.channels[i]);
                            break;
                        }

                    default:
                        {
                            Debug.LogError($"{nameof(UpdateManager)} Found Unknown {nameof(UpdateChannel.UpdateType)} {this.channels[i].Type}.  This channel will be ignored.");
                            break;
                        }
                }
            }

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
            this.UpdateChannels(this.updateChannels, UpdateChannel.UpdateType.Update, Time.deltaTime);

            //// // NOTE [bgish]: The below code is old and should migrate to channel code once we move Bolt Periodic Update node to channels
            //// var realtimeSinceStartup = Time.realtimeSinceStartup;
            //// 
            //// for (int i = 0; i < functions.Count; i++)
            //// {
            ////     var functionCall = functions[i];
            ////     var deltaTime = realtimeSinceStartup - functionCall.LastCallTime;
            //// 
            ////     if (deltaTime > (1.0f / functionCall.CallFrequency))
            ////     {
            ////         functionCall.LastCallTime = realtimeSinceStartup;
            ////         functionCall.Function.Invoke(deltaTime);
            ////         functions[i] = functionCall;
            ////     }
            //// }
        }

        private void FixedUpdate()
        {
            this.UpdateChannels(this.fixedUpdateChannels, UpdateChannel.UpdateType.FixedUpdate, Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            this.UpdateChannels(this.lateUpdateChannels, UpdateChannel.UpdateType.LateUpdate, Time.deltaTime);
        }

        private void UpdateChannels(List<UpdateChannel> channels, UpdateChannel.UpdateType updateType, float deltaTime)
        {
            for (int i = 0; i < channels.Count; i++)
            {
                #if ENABLE_PROFILING
                channels[i].CustomSampler.Begin();
                #endif

                channels[i].Run(deltaTime);

                #if ENABLE_PROFILING
                channels[i].CustomSampler.End();
                #endif
            }
        }
    }
}
