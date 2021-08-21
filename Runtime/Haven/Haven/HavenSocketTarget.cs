#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="HavenSocketTarget.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

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
        [Tooltip("Leave this null/empty if this target can accept any socket interactor.")]
        [SerializeField] private string socketType;
        [SerializeField] private SelectEnterEvent socketedEvent;
        [SerializeField] private bool disableSocketOnSocketed;

        [HideInInspector]
        [SerializeField] private XRBaseInteractable interactable;
#pragma warning restore 0649

        public string SocketType => this.socketType;

        public bool CanSocket(HavenSocketInteractor havenSocketInteractor)
        {
            bool isExclusiveSocket = string.IsNullOrWhiteSpace(this.socketType);
            return havenSocketInteractor != null && (isExclusiveSocket == false || this.socketType == havenSocketInteractor.AcceptedType);
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.interactable);
        }

        private void Awake()
        {
            this.OnValidate();

            this.interactable.selectEntered.AddListener(this.SelectedSwitch);
        }

        private void SelectedSwitch(SelectEnterEventArgs selectEnterEventArgs)
        {
            var socketInteractor = selectEnterEventArgs.interactor as HavenSocketInteractor;

            if (this.CanSocket(socketInteractor) == false)
            {
                return;
            }

            if (this.disableSocketOnSocketed)
            {
                this.ExecuteDelayed(0.1f, () => socketInteractor.socketActive = false);
            }

            this.socketedEvent.Invoke(selectEnterEventArgs);
        }
    }
}

#endif
