//-----------------------------------------------------------------------
// <copyright file="PlayerProximity.cs" company="Lost Signal">
//     Copyright (c) Lost Signal. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Events;

    public class PlayerProximity : MonoBehaviour
    {
        private const string ChannelLow = "PlayerProximity.Low";
        private const string ChannelMedium = "PlayerProximity.Medium";
        private const string ChannelHigh = "PlayerProximity.High";

        private static bool UpdateChannelsCreated;

        #pragma warning disable 0649
        [SerializeField] private Frequency frequency;
        [SerializeField] private Bounds bounds;
        [SerializeField] private UnityEvent onPlayerEnter;
        [SerializeField] private UnityEvent onPlayerExit;
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
            // UpdateManager.OnInitialize(this.Initialize);
        }

        private void Initialize()
        {
            if (UpdateChannelsCreated == false)
            {
                UpdateChannelsCreated = true;

                UpdateManager.Instance.CreateChannel(ChannelLow, 1);
                UpdateManager.Instance.CreateChannel(ChannelMedium, 3);
                UpdateManager.Instance.CreateChannel(ChannelHigh, 10);
            }

            switch (this.frequency)
            {
                case Frequency.Low:
                    {
                        UpdateManager.Instance.Register(ChannelLow, this.PeriodicUpdate);
                        break;
                    }

                case Frequency.Medium:
                    { 
                        UpdateManager.Instance.Register(ChannelMedium, this.PeriodicUpdate);
                        break;
                    }

                case Frequency.High:
                    {
                        UpdateManager.Instance.Register(ChannelHigh, this.PeriodicUpdate);
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
                this.onPlayerEnter?.Invoke();
            }
            else if (this.hasPlayerEntered && isPlayerInside == false)
            {
                this.hasPlayerEntered = false;
                this.onPlayerExit?.Invoke();
            }
        }

        private bool IsInsideBounds()
        {
            return false;
        }
    }
}
