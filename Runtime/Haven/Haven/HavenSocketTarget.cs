//-----------------------------------------------------------------------
// <copyright file="HavenSocketTarget.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using Lost;
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [AddComponentMenu("Haven XR/Socket/HXR Socket Target")]
    [RequireComponent(typeof(XRBaseInteractable))]
    public class HavenSocketTarget : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private string socketType;
        [SerializeField] private XRInteractableEvent socketedEvent;
        [SerializeField] private bool disableSocketOnSocketed;
#pragma warning restore 0649

        public string SocketType => this.socketType;

        private void Awake()
        {
            var interactable = this.GetComponent<XRBaseInteractable>();
            interactable.onSelectEntered.AddListener(SelectedSwitch);
        }

        private void SelectedSwitch(XRBaseInteractor interactor)
        {
            var socketInteractor = interactor as HavenExclusiveSocketInteractor;

            if (socketInteractor == null || this.socketType != socketInteractor.AcceptedType)
            {
                return;
            }

            if (this.disableSocketOnSocketed)
            {
                this.ExecuteDelayed(0.5f, () => socketInteractor.socketActive = false);
            }

            this.socketedEvent.Invoke(interactor);
        }
    }
}

#endif
