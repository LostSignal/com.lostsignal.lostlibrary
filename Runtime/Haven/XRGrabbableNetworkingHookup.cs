#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="XRGrabbableNetworkingHookup.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
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

            this.xrGrabInteractable.selectEntered.AddListener(this.OnSelectEntered);
            this.xrGrabInteractable.selectExited.AddListener(this.OnSelectExited);
        }

        private void OnSelectEntered(SelectEnterEventArgs _)
        {
            if (Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                this.networkIdentity.RequestOwnership();
            }
        }

        private void OnSelectExited(SelectExitEventArgs _)
        {
            if (Time.realtimeSinceStartup - this.awakeTime > 1.0f)
            {
                this.networkIdentity.ReleaseOwnership();
            }
        }
    }
}

#endif
