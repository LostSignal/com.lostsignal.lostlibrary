//-----------------------------------------------------------------------
// <copyright file="CameraShake.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    ////
    //// NOTE [bgish]: This is really old code, should eventually delete this in favor of just using Cinemachine
    ////
    public sealed class CameraShake : LoadBalancedMonoBehaviour
    {
        private static readonly ObjectTracker<CameraShake> Shakers = new ObjectTracker<CameraShake>(5);
        private static readonly string CameraShakeUpdateChannel = "CameraShake";

#pragma warning disable 0649
        [SerializeField] private float shakeTime = 0.25f;
        [SerializeField] private float shakeAmount = 0.2f;

        [HideInInspector]
        [SerializeField] private Transform myTransform;
#pragma warning restore 0649

        private float currentShakeTime;
        private Vector3 originalPosition;
        private bool isInitialized;

        public override string Name => nameof(CameraShake);

        public override bool RunAwake => true;

        public override bool RunStart => false;

        public static void Shake()
        {
            for (int i = 0; i < Shakers.Count; i++)
            {
                Shakers[i].PrivateShake();
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if (this.currentShakeTime > 0.0f)
            {
                this.myTransform.localPosition = this.originalPosition + (Random.insideUnitSphere * this.shakeAmount);
                this.currentShakeTime -= deltaTime;
            }
            else
            {
                this.myTransform.localPosition = this.originalPosition;
                this.StopUpdating();
            }
        }

        protected override void LoadBalancedAwake()
        {
            this.OnValidate();
            this.isInitialized = true;
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.myTransform);
        }

        private void PrivateShake()
        {
            if (this.isInitialized == false)
            {
                return;
            }

            this.currentShakeTime = this.shakeTime;
            this.originalPosition = this.myTransform.localPosition;
            this.StartUpadating(CameraShakeUpdateChannel);
        }

        private void OnEnable()
        {
            Shakers.Add(this);
        }

        private void OnDisable()
        {
            Shakers.Remove(this);
        }
    }
}

#endif
