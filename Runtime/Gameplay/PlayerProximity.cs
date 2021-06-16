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

        private static bool AreStaticsInitialized;
        private static Channel ChannelLow;
        private static Channel ChannelMedium;
        private static Channel ChannelHigh;

        static PlayerProximity()
        {
            Bootloader.OnReset += Reset;

            void Reset()
            {
                AreStaticsInitialized = false;
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

        private bool hasPlayerEntered;

        public enum Frequency
        {
            Low,
            Medium,
            High,
        }

        private void Awake()
        {
            // CAN THE BOOTLOADER SYSTEM INSURE THIS ALWAYS WORKS?
            AwakeManager.Instance.QueueWork(this.Initialize, "PlayerProximity.Awake", this);

            Bootloader.OnManagersReady += this.Initialize;
            // UpdateManager.OnInitialize(this.Initialize);


        }

        private void Initialize()
        {
            if (AreStaticsInitialized == false)
            {
                AreStaticsInitialized = true;

                ChannelLow = UpdateManager.Instance.GetOrCreateChannel(ChannelLowName, 100, 1);
                ChannelMedium = UpdateManager.Instance.GetOrCreateChannel(ChannelMediumName, 100, 3);
                ChannelHigh = UpdateManager.Instance.GetOrCreateChannel(ChannelHighName, 100, 10);
            }

            switch (this.frequency)
            {
                case Frequency.Low:
                    {
                        ChannelLow.AddCallback(this.PeriodicUpdate, "PlayerProximity.Low", this);
                        break;
                    }

                case Frequency.Medium:
                    { 
                        ChannelMedium.AddCallback(this.PeriodicUpdate, "PlayerProximity.Medium", this);
                        break;
                    }

                case Frequency.High:
                    {
                        ChannelHigh.AddCallback(this.PeriodicUpdate, "PlayerProximity.High", this);
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
