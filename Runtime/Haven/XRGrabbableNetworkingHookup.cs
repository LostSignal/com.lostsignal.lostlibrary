//-----------------------------------------------------------------------
// <copyright file="XRGrabbableNetworkingHookup.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace HavenXR
{
    using Lost.Networking;
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    public class XRGrabbableNetworkingHookup : MonoBehaviour
    {
        private NetworkIdentity networkIdentity;
        private XRBaseInteractable xrGrabInteractable;
        private float awakeTime;

        private void Awake()
        {
            this.awakeTime = Time.realtimeSinceStartup;
            this.networkIdentity = this.GetComponent<NetworkIdentity>();

            if (this.xrGrabInteractable == null)
            {
                this.xrGrabInteractable = this.GetComponent<XROffsetGrabbable>();
            }

            if (this.xrGrabInteractable == null)
            {
                this.xrGrabInteractable = this.GetComponent<XRGrabInteractable>();
            }

            if (this.xrGrabInteractable == null)
            {
                this.xrGrabInteractable = this.GetComponent<BetterXRGrabInteractable>();
            }

            if (!this.networkIdentity || !this.xrGrabInteractable)
            {
                Debug.LogError($"{this.name} has XRGrabHookup component, but invalid NetworkIdentity or XRGrabInteractable component(s).", this);
                return;
            }
            else if (this.networkIdentity.CanChangeOwner == false)
            {
                Debug.LogError($"{this.name} has XRGrabHookup component, but CanChangeOwner = false.", this);
            }

            this.xrGrabInteractable.onSelectEntered.AddListener(this.OnSelectEntered);
            this.xrGrabInteractable.onSelectExited.AddListener(this.OnSelectExited);
        }

        private void OnSelectEntered(XRBaseInteractor _)
        {
            if (Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                this.networkIdentity.RequestOwnership();
            }
        }

        private void OnSelectExited(XRBaseInteractor _)
        {
            if (Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                this.networkIdentity.ReleaseOwnership();
            }
        }
    }
}

#endif
