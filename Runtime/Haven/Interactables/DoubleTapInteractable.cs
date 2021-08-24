//-----------------------------------------------------------------------
// <copyright file="DoubleTapInteractable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace HavenXR
{
    using System;
    using Lost;
    using UnityEngine;
    using UnityEngine.Events;

    public class DoubleTapInteractable : Interactable
    {
        #pragma warning disable 0649
        [SerializeField] private RaycastHitUnityEvent doubleTappedEvent = new RaycastHitUnityEvent();
        [SerializeField] private float doubleTapTimeLength = 0.33f;
        #pragma warning restore 0649

        private float lastTapTimeSinceStartup;

        public UnityEvent<RaycastHit> DoubleTappedEvent
        {
            get { return this.doubleTappedEvent; }
        }

        protected override void OnInput(Lost.Input input, Collider collider, Camera camera)
        {
            if (input.InputState == InputState.Released)
            {
                RaycastHit hit;

                if (this.doubleTappedEvent != null && collider.Raycast(camera.ScreenPointToRay(input.CurrentPosition), out hit, float.MaxValue))
                {
                    float newTapTime = Time.realtimeSinceStartup;

                    if (newTapTime - this.lastTapTimeSinceStartup < this.doubleTapTimeLength)
                    {
                        this.doubleTappedEvent.Invoke(hit);
                        this.lastTapTimeSinceStartup = 0;
                    }
                    else
                    {
                        this.lastTapTimeSinceStartup = newTapTime;
                    }
                }
            }
        }

        [Serializable]
        public class RaycastHitUnityEvent : UnityEvent<RaycastHit>
        {
        }
    }
}

#endif
