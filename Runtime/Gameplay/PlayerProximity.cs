//-----------------------------------------------------------------------
// <copyright file="PlayerProximity.cs" company="Lost Signal">
//     Copyright (c) Lost Signal. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Events;

    ////
    //// TODO [bgish]: Combine OnPlayerEnterExit with this class to be more generic, then remove OnPlayerEnterExit
    ////
    public class PlayerProximity : MonoBehaviour
    {
        private const string ChannelLowName = "PlayerProximity.Low";
        private const string ChannelMediumName = "PlayerProximity.Medium";
        private const string ChannelHighName = "PlayerProximity.High";

        private static UpdateChannel ChannelLow;
        private static UpdateChannel ChannelMedium;
        private static UpdateChannel ChannelHigh;

        static PlayerProximity()
        {
            Bootloader.OnReset += Reset;

            void Reset()
            {
                ChannelLow = null;
                ChannelMedium = null;
                ChannelHigh = null;
            }
        }

        #pragma warning disable 0649
        [SerializeField] private Frequency frequency;
        [SerializeField] private Bounds bounds;
        [SerializeField] private UnityEvent onEnterProximity;
        [SerializeField] private UnityEvent onExitProximity;
        #pragma warning restore 0649
        
        private CallbackReceipt callbackReceipt;
        private bool hasPlayerEntered;

        public enum Frequency
        {
            Low,
            Medium,
            High,
        }

        private void Awake()
        {
            AwakeManager.Instance.QueueWork(this.Initialize, "PlayerProximity.Awake", this);
        }

        private void OnDestroy()
        {
            this.callbackReceipt.Cancel();
        }

        private void Initialize()
        {
            if (ChannelLow == null)
            {
                ChannelLow = UpdateManager.Instance.GetOrCreateChannel(ChannelLowName, 100, 1.0f);
                ChannelMedium = UpdateManager.Instance.GetOrCreateChannel(ChannelMediumName, 100, 0.33f);
                ChannelHigh = UpdateManager.Instance.GetOrCreateChannel(ChannelHighName, 100, 0.0f);
            }

            switch (this.frequency)
            {
                case Frequency.Low:
                    {
                        ChannelLow.RegisterCallback(ref this.callbackReceipt, this.PeriodicUpdate, this);
                        break;
                    }

                case Frequency.Medium:
                    { 
                        ChannelMedium.RegisterCallback(ref this.callbackReceipt, this.PeriodicUpdate, this);
                        break;
                    }

                case Frequency.High:
                    {
                        ChannelHigh.RegisterCallback(ref this.callbackReceipt, this.PeriodicUpdate, this);
                        break;
                    }

                default:
                    {
                        Debug.LogError($"PlayerProximity encountered unknown frequency type {this.frequency}.", this);
                        break;
                    }
            }
        }

        private void PeriodicUpdate(float deltaTime)
        {
            bool isPlayerInside = this.IsInsideBounds();

            if (this.hasPlayerEntered == false && isPlayerInside)
            {
                this.hasPlayerEntered = true;
                this.onEnterProximity?.Invoke();
            }
            else if (this.hasPlayerEntered && isPlayerInside == false)
            {
                this.hasPlayerEntered = false;
                this.onExitProximity?.Invoke();
            }
        }

        private bool IsInsideBounds()
        {
            return false;
        }
    }
}
