//-----------------------------------------------------------------------
// <copyright file="HavenInteractorEventDispatcher.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Haven
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [AddComponentMenu("Haven XR/HXR Interactor Event Dispatcher")]
    [RequireComponent(typeof(XRBaseInteractor))]
    public class HavenInteractorEventDispatcher : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private XRInteractorEvent onSelectedEnter;
#pragma warning restore 0649

        private void Awake()
        {
            var interactor = this.GetComponent<XRBaseInteractor>();
            interactor.onSelectEntered.AddListener(interactable => { this.onSelectedEnter.Invoke(interactable); });
        }
    }
}

#endif