//-----------------------------------------------------------------------
// <copyright file="TapInteractable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace HavenXR
{
    using System;
    using Lost;
    using UnityEngine;
    using UnityEngine.Events;

    public class TapInteractable : Interactable
    {
        #pragma warning disable 0649
        [SerializeField] private RaycastHitUnityEvent tappedEvent = new RaycastHitUnityEvent();
        #pragma warning restore 0649

        public UnityEvent<RaycastHit> TappedEvent
        {
            get { return this.tappedEvent; }
        }

        protected override void OnInput(Lost.Input input, Collider collider, Camera camera)
        {
            if (input.InputState == InputState.Released)
            {
                RaycastHit hit;

                if (this.tappedEvent != null && collider.Raycast(camera.ScreenPointToRay(input.CurrentPosition), out hit, float.MaxValue))
                {
                    this.tappedEvent.Invoke(hit);
                }
            }
        }

        [Serializable]
        public class RaycastHitUnityEvent : UnityEvent<RaycastHit>
        {
        }
    }
}
