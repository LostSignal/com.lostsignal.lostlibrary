//-----------------------------------------------------------------------
// <copyright file="OnPlayerEnterExit.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class OnPlayerEnterExit : MonoBehaviour
    {
        private static UpdateChannel highPriorityChannel;
        private static UpdateChannel lowPriorityChannel;

        //// private static bool AreStaticsInitialized;

        // Reset is called on reboot and leaving editor mode
        static OnPlayerEnterExit()
        {
            Bootloader.OnReset += Reset;

            void Reset()
            {
                highPriorityChannel = default(UpdateChannel);
                lowPriorityChannel = default(UpdateChannel);
                //// AreStaticsInitialized = false;
            }
        }

#pragma warning disable 0649
        [SerializeField] private Bounds onEnterBounds;
        [SerializeField] private bool playerMustBeFacing;

        [Header("High Priority")]
        [SerializeField] private bool switchToHighPriority;
        [SerializeField] private Bounds highPriorityBounds;

        [Header("Events")]
        [SerializeField] private UnityEvent onPlayerEnter;
        [SerializeField] private UnityEvent onPlayerExit;
#pragma warning restore 0649

        private UpdateChannelReceipt receipt;
        private bool isPlayerInside;

        private UpdateChannel CurrentChannel
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.isPlayerInside ? highPriorityChannel : lowPriorityChannel;
        }

        private void Awake()
        {
            Debug.Log("OnPlayerEnterExit: OnPlayerEnterExit.Awake");
            Bootloader.OnManagersReady += Initialize;

            void Initialize()
            {
                Debug.Log("OnPlayerEnterExit: OnPlayerEnterExit.Initialize");
                this.UpdateRegistationWithUpdateManager();
                this.isPlayerInside = this.GetIsPlayerInside();
                this.InvokeEvent(this.isPlayerInside);
            }
        }

        private void OnEnable()
        {
            this.UpdateRegistationWithUpdateManager();
        }

        private void OnDisable()
        {
            this.UpdateRegistationWithUpdateManager();
        }

        private void UpdateRegistationWithUpdateManager()
        {
            // // Making sure our static variables are initialized
            // if (AreStaticsInitialized == false && UpdateManager.IsInitialized)
            // {
            //     AreStaticsInitialized = true;
            //     HighPriorityChannel = UpdateManager.Instance.GetOrCreateChannel("OnPlayerEnterExit_High", Priority.High);
            //     LowPriorityChannel = UpdateManager.Instance.GetOrCreateChannel("OnPlayerEnterExit_Low", Priority.Low);
            // }
            //
            // if (AreStaticsInitialized == false)
            // {
            //     return;
            // }
            // else if (this != null && this.enabled)
            // {
            //     this.CurrentChannel.Register(ref this.receipt, this.DoWork, "Description...", this);
            // }
            // else
            // {
            //     this.CurrentChannel.Unregister(ref this.receipt);
            // }
        }

        private void OnDrawGizmo()
        {
            // Draw both bounds with different colors
        }

        // [ForceInline]
        private bool GetIsPlayerInside()
        {
            //// bool isInside = this.onPlayerEnterBounds.Contains(PlayerManager.Instance.PlayePosition);
            ////
            //// if (isInside && this.playerMustBeFacing)
            //// {
            ////     Vector3 thisObjectsForward = this.transform.position - CharacterManager.Instance.MainPlayer.Position);
            ////     return Vector3.Dot(CharacterManager.Instance.MainPlayer.Forward, thisObjectsForward) > CosUtil.Cos45;
            //// }
            ////
            //// return isInside;

            return false;
        }

        private void DoWork()
        {
            // bool currentIsPlayerInside = this.GetIsPlayerInside();
            //
            // if (this.isPlayerInside != currentIsPlayerInside)
            // {
            //     this.isPlayerInside = currentIsPlayerInside;
            //     this.InvokeEvent(this.isPlayerInside);
            // }
            //
            // // Detecting if we need to change our channel
            // var currentChannel = this.CurrentChannel;
            //
            // if (receipt.Channel != currentChannel)
            // {
            //     currentChannel.SwitchToThisChannel(ref this.receipt);
            // }
        }

        private void InvokeEvent(bool isPlayerInside)
        {
            // if (isPlayerInside)
            // {
            //     this.onPlayerenter.SafeInvoke();
            // }
            // else
            // {
            //     this.onPlayerExit.SafeInvoke();
            // }
        }
    }
}

#endif
