#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="HavenExclusiveSocketInteractor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [AddComponentMenu("Haven XR/Socket/HXR Socket Interactor")]
    public class HavenSocketInteractor : XRSocketInteractor
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

            return base.CanSelect(interactable) && socketTarget.CanSocket(this);
        }

        public override bool CanHover(XRBaseInteractable interactable)
        {
            return this.CanSelect(interactable);
        }
    }
}

#endif
