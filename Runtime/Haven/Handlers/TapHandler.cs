//-----------------------------------------------------------------------
// <copyright file="TapHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace HavenXR
{
    using UnityEngine;

    [RequireComponent(typeof(TapInteractable))]
    public class TapHandler : MonoBehaviour
    {
        private TapInteractable tapInteractable;

        private void Awake()
        {
            this.tapInteractable = this.GetComponent<TapInteractable>();
            this.tapInteractable.TappedEvent.AddListener(this.Tapped);
        }

        private void OnDestroy()
        {
            this.tapInteractable.TappedEvent.RemoveListener(this.Tapped);
        }

        private void Tapped(RaycastHit hit)
        {
            Debug.LogFormat("{0} was tapped", this.name);
        }
    }
}
