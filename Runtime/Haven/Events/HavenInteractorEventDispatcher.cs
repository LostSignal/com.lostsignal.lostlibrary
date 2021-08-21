#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="HavenInteractorEventDispatcher.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

//// NOTE [bgish]: WHAT IS THE PURPOSE OF THIS????  Remoe???

namespace Lost.Haven
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [AddComponentMenu("Haven XR/HXR Interactor Event Dispatcher")]
    [RequireComponent(typeof(XRBaseInteractor))]
    public class HavenInteractorEventDispatcher : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private SelectEnterEvent onSelectedEnter;
#pragma warning restore 0649

        private void Awake()
        {
            var interactor = this.GetComponent<XRBaseInteractor>();
            interactor.selectEntered.AddListener(selectEnterEventArgs => { this.onSelectedEnter.Invoke(selectEnterEventArgs); });
        }
    }
}

#endif
