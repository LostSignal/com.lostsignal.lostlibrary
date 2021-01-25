//-----------------------------------------------------------------------
// <copyright file="HavenRayScaler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace HavenXR
{
    using Lost;
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [RequireComponent(typeof(XRRayInteractor))]
    public class HavenRayScaler : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private AnimationCurve rigScaleMultiplierCurve;
        [SerializeField] private XRRayInteractor xrRayInteractor;
#pragma warning restore 0649

        private HavenRig havenRig;

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.xrRayInteractor);
        }

        private void Awake()
        {
            this.OnValidate();
            this.havenRig = this.GetComponentInParent<HavenRig>();
        }

        private void Update()
        {
            float dot = Mathf.Clamp01(Vector3.Dot(this.transform.forward, Vector3.up));
            this.xrRayInteractor.velocity = this.scaleCurve.Evaluate(dot) * this.rigScaleMultiplierCurve.Evaluate(this.havenRig.RigScale);
        }
    }
}

#endif
