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
    public class CameraShake : MonoBehaviour
    {
        private static ComponentTracker<CameraShake> Shakers = new ComponentTracker<CameraShake>(5);

#pragma warning disable 0649
        [SerializeField] private float shakeTime = 0.25f;
        [SerializeField] private float shakeAmount = 0.2f;

        [HideInInspector] 
        [SerializeField] private Transform myTransform;
#pragma warning restore 0649

        private bool isInitialized;
        private UpdateChannel updateChannel;
        private CallbackReceipt receipt;
        private float currentShakeTime;
        private Vector3 originalPosition;
        
        public static void Shake()
        {
            for (int i = 0; i < Shakers.Count; i++)
            {
                Shakers[i].PrivateShake();
            }
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.myTransform);
        }

        private void Awake()
        {
            AwakeManager.Instance.QueueWork(this.Initialize, "CameraShake.Awake", this);
        }

        private void Initialize()
        {
            if (this.isInitialized == false)
            {
                this.isInitialized = true;
                this.OnValidate();
                this.updateChannel = UpdateManager.Instance.GetOrCreateChannel("Camera Shake", 5);
            }
        }

        private void PrivateShake()
        {
            this.Initialize();
            this.currentShakeTime = this.shakeTime;
            this.originalPosition = this.myTransform.localPosition;
            this.updateChannel.RegisterCallback(ref this.receipt, this.DoWork, this);
        }

        private void DoWork(float deltaTime)
        {
            if (this.currentShakeTime > 0.0f)
            {
                this.myTransform.localPosition = this.originalPosition + (Random.insideUnitSphere * this.shakeAmount);
                this.currentShakeTime -= deltaTime;
            }
            else
            {
                this.myTransform.localPosition = this.originalPosition;
                this.receipt.Cancel();
            }
        }

        private void OnEnable()
        {
            Shakers.Add(this);
        }

        private void OnDisable()
        {
            Shakers.Remove(this);
            this.receipt.Cancel();
        }
    }
}

#endif
