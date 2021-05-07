//-----------------------------------------------------------------------
// <copyright file="HavenOffsetGrabbable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using Networking;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.XR.Interaction.Toolkit;

    [AddComponentMenu("Haven XR/Interactables/HXR Grabbable")]
    public class HavenGrabbable : BetterXRGrabInteractable
    {
#pragma warning disable 0649
        [Header("Haven Variables")]
        [SerializeField] private bool isOffsetGrabbable = true;
        
        [Header("Hover")]
        [SerializeField] private UnityEvent onHoverStart;
        [SerializeField] private UnityEvent onHoverStop;
        
        [Header("Grab")]
        [SerializeField] private UnityEvent onGrabStart;
        [SerializeField] private UnityEvent onGrabStop;

        [Header("Use")]
        [SerializeField] private UnityEvent onUseStart;
        [SerializeField] private UnityEvent onUseStop;
#pragma warning restore 0649

        private NetworkIdentity networkIdentity;
        private bool usingNetworking;
        private float originalTightenPosition;
        private float awakeTime;

        private bool isBeingHovered;
        private bool isBeingGrabbed;
        private bool isBeingUsed;

        public bool IsBeingHovered => this.isBeingHovered;

        public bool IsBeingGrabbed => this.isBeingGrabbed;

        public bool IsBeingUsed => this.isBeingUsed;

        public bool IsOffsetGrabbable
        {
            get => this.isOffsetGrabbable;
            set => this.isOffsetGrabbable = value;
        }

        public override bool IsSelectableBy(XRBaseInteractor interactor)
        {
            int interactorLayerMask = 1 << interactor.gameObject.layer;
            return base.IsSelectableBy(interactor) && (interactionLayerMask.value & interactorLayerMask) != 0;
        }

        protected override void Awake()
        {
            base.Awake();

            this.networkIdentity = this.GetComponent<NetworkIdentity>();
            this.usingNetworking = this.networkIdentity != null;
            this.originalTightenPosition = this.tightenPosition;
            this.awakeTime = Time.realtimeSinceStartup;
            
            this.firstHoverEntered.AddListener(this.HoverStart);
            this.lastHoverExited.AddListener(this.HoverStop);
            this.selectEntered.AddListener(this.GrabStart);
            this.selectExited.AddListener(this.GrabStop);
            this.activated.AddListener(this.UseStart);
            this.deactivated.AddListener(this.UseStop);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            if (this.isOffsetGrabbable)
            {
                if (this.attachTransform == null)
                {
                    this.attachTransform = new GameObject("Attach Transform").transform;
                    this.attachTransform.SetParent(this.transform);
                    this.attachTransform.localPosition = Vector3.zero;
                    this.attachTransform.localRotation = Quaternion.identity;
                    this.attachTransform.localScale = Vector3.one;
                }

                this.attachTransform.position = selectEnterEventArgs.interactor.attachTransform.position;

                // Fixing bug with tighten position and direct interactors
                bool isDirect = selectEnterEventArgs.interactor is XRDirectInteractor;
                this.tightenPosition = isDirect ? 1 : this.originalTightenPosition;
            }

            if (this.usingNetworking && Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                this.networkIdentity.RequestOwnership();
            }

            base.OnSelectEntered(selectEnterEventArgs);
        }

        protected override void OnSelectExited(SelectExitEventArgs selectExitedEventArgs)
        {
            if (this.usingNetworking && Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                this.networkIdentity.ReleaseOwnership();
            }

            base.OnSelectExited(selectExitedEventArgs);
        }
        
        private void HoverStart(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (this.isBeingGrabbed == false)
            {
                this.onHoverStart?.Invoke();
                this.isBeingHovered = false;
            }
        }

        private void HoverStop(HoverExitEventArgs hoverExitEventArgs)
        {
            if (this.isBeingGrabbed == false)
            {
                this.isBeingHovered = true;
                this.onHoverStop?.Invoke();
            }
        }

        private void GrabStart(SelectEnterEventArgs selectEnterEventArgs)
        {
            this.isBeingGrabbed = true;
            this.onGrabStart?.Invoke();
        }

        private void GrabStop(SelectExitEventArgs selectExitEventArgs)
        {
            // NOTE [bgish]: If we're using it, but let go of it, make sure onUseStop event is fired
            if (this.isBeingUsed)
            {
                this.UseStop(null);
            }

            this.onGrabStop?.Invoke();
            this.isBeingGrabbed = false;
        }

        private void UseStart(ActivateEventArgs activateEventArgs)
        {
            this.isBeingUsed = true;
            this.onUseStart?.Invoke();
        }

        private void UseStop(DeactivateEventArgs deactivateEventArgs)
        {
            this.onUseStop?.Invoke();
            this.isBeingUsed = false;
        }
    }
}

#endif
