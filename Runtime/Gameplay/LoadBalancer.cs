#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LoadBalancer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    //// MOST THE CODE HAS BEEN MOVED TO AwakeManager.cs, this class should probably go away
    ////
    //// using System;
    //// using System.Collections.Generic;
    //// using UnityEngine;
    ////
    //// public class LoadBalancer
    //// {
    ////     private static readonly Callback DefaultCallback = default(Callback);
    ////
    ////     private static Channel LoadBalancerChannel = null;
    ////     private static bool AreStaticsInitialized = false;
    ////
    ////     static LoadBalancer()
    ////     {
    ////         Bootloader.OnReset += Reset;
    ////
    ////         void Reset()
    ////         {
    ////             LoadBalancerChannel = default(Channel);
    ////             AreStaticsInitialized = false;
    ////         }
    ////     }
    ////
    ////     private static void InitializeStatics()
    ////     {
    ////         if (AreStaticsInitialized == false && UpdateManager.IsInitialized)
    ////         {
    ////             LoadBalancerChannel = UpdateManager.Instance.GetOrCreateChannel("LoadBalancer", 1000);
    ////             AreStaticsInitialized = true;
    ////         }
    ////     }
    ////
    ////     public struct Callback
    ////     {
    ////         public Action Action;
    ////
    ////         #if UNITY_EDITOR || DEVELOPMENT_BUILD
    ////         public float QueueTime;              // Development Build Only
    ////         public UnityEngine.Object Context;   // Development Build Only
    ////         public string Description;           // Development Build Only
    ////         #endif
    ////     }
    ////
    ////     #pragma warning disable 0649
    ////     [SerializeField] private List<Callback> callbacks;
    ////     #pragma warning restore 0649
    ////
    ////     // Make sure UnityEditor sets the capicty of these in OnValidate so we serialize it and not create it at runtime.
    ////     public LoadBalancer(int capacity)
    ////     {
    ////         this.callbacks = new List<Callback>(capacity);
    ////     }
    ////
    ////     int startIndex = 0;
    ////     int endIndex = 0;
    ////
    ////     public void Pause()
    ////     {
    ////
    ////     }
    ////
    ////     public void Unpause()
    ////     {
    ////
    ////     }
    ////
    ////     public enum State
    ////     {
    ////         Paused,
    ////         Processing,
    ////         Idle,
    ////     }
    ////
    ////     public void QueueWork(System.Action action, string description = null, UnityEngine.Object context = null)
    ////     {
    ////         // DefaultCallback.a
    ////         // Print error if callbacks list needs to grow, suggest increases starting size
    ////     }
    ////
    ////     public void ScheduleWork(float maxRuntime)
    ////     {
    ////         // Goes backwards through the simple callbacks list till maxRuntime is hit
    ////         // Print warnings if one action invoke takes longer than max alloted runtime (using description and context)
    ////     }
    ////
    ////     private void DoWork(float deltaTime)
    ////     {
    ////
    ////     }
    //// }
    ////
    //// //// public struct SimpleCallback
    //// //// {
    //// ////     Action Action;
    //// ////
    //// ////     #if UNITY_EDITOR || DEVELOPMENT_BUILD
    //// ////     float TotalRuntime;           // Development Build Only
    //// ////     UnityEngine.Object Context;   // Development Build Only
    //// ////     string Description;           // Development Build Only
    //// ////     #endif
    //// //// }
    //// ////
    //// //// public struct LongRunningCallback
    //// //// {
    //// ////     bool IsActive;
    //// ////     Func<bool> Func;
    //// ////     Action OnComplete;
    //// ////
    //// ////     #if UNITY_EDITOR || DEVELOPMENT_BUILD
    //// ////     float TotalRuntime;
    //// ////     int TotalCallCount;
    //// ////     UnityEngine.Object Context;
    //// ////     string Description;
    //// ////     float StartTime;
    //// ////     float EndTime;
    //// ////     #endif
    //// //// }
    //// ////
    //// //// [Serializable]
    //// //// public class SimpleLoadBalancer
    //// //// {
    //// ////     [SerializeField] private List<SimpleCallback> callbacks;
    //// ////
    //// ////     // Make sure UnityEditor sets the capicty of these in OnValidate so we serialize it and not create it at runtime.
    //// ////     public SimpleLoadBalancer(int capacity)
    //// ////     {
    //// ////         this.callbacks = new List<SimpleCallback>(capacity);
    //// ////     }
    //// ////
    //// ////     public void QueueWork(System.Action action, string description)
    //// ////     {
    //// ////         // Print error if callbacks list needs to grow, suggest increases starting size
    //// ////     }
    //// ////
    //// ////     public void DoWork(float maxRuntime)
    //// ////     {
    //// ////         // Goes backwards through the simple callbacks list till maxRuntime is hit
    //// ////         // Print warnings if one action invoke takes longer than max alloted runtime (using description and context)
    //// ////     }
    //// //// }
    //// ////
    //// //// [Serializable]
    //// //// public class LongRunningLoadBalancer
    //// //// {
    //// ////     [SerializeField] private List<LongRunningCallback> longRunningCallbacks;
    //// ////
    //// ////    // Make sure UnityEditor sets the capicty of these in OnValidate so we serialize it and not create it at runtime.
    //// ////     public LongRunningLoadBalancer(int capacity)
    //// ////     {
    //// ////         this.longRunningCallbacks = new List<LongRunningCallback>(capacity);
    //// ////     }
    //// ////
    //// ////     public void QueueWork(Func<bool> func, string description)
    //// ////     {
    //// ////         // Print error if callbacks needs to grow, suggest increases starting size
    //// ////     }
    //// ////
    //// ////     public void DoWork(float maxRuntime)
    //// ////     {
    //// ////         // Goes backwards through the long running callbacks list till maxLongRunningRuntime is hit or the function returns true and it goes to the next item.
    //// ////
    //// ////         // If item is removed, then replace it's spot in the list with the last element in the list
    //// ////
    //// ////         // Print warnings if one action invoke takes longer than max alloted runtime (using description)
    //// ////         // Keep track of statistics like runtime by function
    //// ////         // average updates to complete function
    //// ////     }
    //// //// }
    //// ////
    //// //// public class LoadBalancerManager : Manager<UpdateManager>
    //// //// {
    //// ////     // Has SimpleLoadBalancer
    //// ////     // Has LongRunningLoadBalancer
    //// ////     // Uses the UpdateManager Channel System to update it's work
    //// ////     // Classes interface with this class instead of the SimpleLoadBalancer or LongRunningLoadBalancer
    //// ////     // This class is resonsible for making sure the default capacities are setup correctly
    //// //// }
}
