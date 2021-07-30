//-----------------------------------------------------------------------
// <copyright file="PlayerProximity.cs" company="Lost Signal">
//     Copyright (c) Lost Signal. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Events;

    ////
    //// TODO [bgish]: Combine OnPlayerEnterExit with this class to be more generic, then remove OnPlayerEnterExit
    ////
    public sealed class PlayerProximity : LoadBalancedMonoBehaviour
    {
        // private bool enterHighPriBasedOnDistance;
        // private bool radius;

        // private bool enterHighPriBasedOnView;
        // private bool viewingAngle;

        private static readonly string[] PlayerProximityChannels = new string[]
        {
            "PlayerProximity.Low",
            "PlayerProximity.Medium",
            "PlayerProximity.High",
        };

        public enum Frequency
        {
            Low = 0,
            Medium = 1,
            High = 2,
        }

#pragma warning disable 0649
        [SerializeField] private Frequency defaultFrequency;
        [SerializeField] private Bounds bounds;
        [SerializeField] private UnityEvent onEnterProximity;
        [SerializeField] private UnityEvent onExitProximity;
#pragma warning restore 0649
        
        private string currentUpdateChannelName;
        private bool hasPlayerEntered;

        public override string Name => nameof(PlayerProximity);
        
        public override bool RunAwake => true;

        public override bool RunStart => false;

        protected override void LoadBalancedAwake()
        {
            this.DoUpdate(0.0f);
        }

        public override void DoUpdate(float deltaTime)
        {
            bool isPlayerInside = this.IsInProximity();
            
            // Determining Whether to fire events
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

            // Figuring out if the update channel needs to change based on player position / facing
            Frequency frequency = this.defaultFrequency;

            if (frequency != Frequency.High)
            {
                frequency = this.IsInHighProximity() ? Frequency.High : frequency;
            }

            string newUpdateChanel = PlayerProximityChannels[(int)frequency];
            if (this.currentUpdateChannelName != newUpdateChanel)
            {
                this.currentUpdateChannelName = newUpdateChanel;
                this.StartUpadating(newUpdateChanel);
            }
        }

        private bool IsInProximity()
        {
            return false;
        }

        private bool IsInHighProximity()
        {
            return false;
        }
    }
}

#endif
