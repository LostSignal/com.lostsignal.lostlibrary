//-----------------------------------------------------------------------
// <copyright file="HavenExclusiveSocketInteractor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [AddComponentMenu("Haven XR/Socket/HXR Exclusive Socket Interactor")]
    public class HavenExclusiveSocketInteractor : XRSocketInteractor
    {
#pragma warning disable 0649
        [SerializeField] private string acceptedType;
#pragma warning restore 0649

        public string AcceptedType => this.acceptedType;

        public override bool CanSelect(XRBaseInteractable interactable)
        {
            var socketTarget = interactable.GetComponent<HavenSocketTarget>();

            if (socketTarget == null)
            {
                return false;
            }

            return base.CanSelect(interactable) && (socketTarget.SocketType == this.acceptedType);
        }

        public override bool CanHover(XRBaseInteractable interactable)
        {
            return this.CanSelect(interactable);
        }
    }
}

#endif
