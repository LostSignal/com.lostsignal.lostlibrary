//-----------------------------------------------------------------------
// <copyright file="HavenOffsetGrabbable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    // TODO [bgish]: Detect if Has a network Transform and if so, hook up the RequestOwnership/ReleaseOwnership fucntions.
    //               See the XRGrabbableNetworkingHookup class, and remove when done with it.

    [AddComponentMenu("Haven XR/Interactables/HXR Grabbable")]
    public class HavenGrabbable : BetterXRGrabInteractable
    {
#pragma warning disable 0649
        [SerializeField] private bool isOffsetGrabbable = true;
#pragma warning restore 0649

        private float originalTightenPosition;
        private float awakeTime;

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

            this.originalTightenPosition = this.tightenPosition;
            this.awakeTime = Time.realtimeSinceStartup;
        }

        protected override void OnSelectEntering(XRBaseInteractor interactor)
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

                this.attachTransform.position = interactor.attachTransform.position;
            }

            base.OnSelectEntering(interactor);
        }

        protected override void OnSelectEntered(XRBaseInteractor interactor)
        {
            if (this.isOffsetGrabbable)
            {
                bool isDirect = interactor is XRDirectInteractor;
                this.tightenPosition = isDirect ? 1 : this.originalTightenPosition;
            }

            if (Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                // TODO [bgish]: Get this component and cache it in Awake
                // this.networkIdentity.RequestOwnership();
            }

            base.OnSelectEntered(interactor);
        }

        protected override void OnSelectExited(XRBaseInteractor interactor)
        {
            if (Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                // TODO [bgish]: Get this component and cache it in Awake
                // this.networkIdentity.ReleaseOwnership();
            }

            base.OnSelectExited(interactor);
        }
    }
}

#endif
