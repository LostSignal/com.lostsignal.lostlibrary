//-----------------------------------------------------------------------
// <copyright file="GPSAvatar.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_CINEMACHINE

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class GPSAvatar : MonoBehaviour
    {
#pragma warning disable 0649
        [Header("Speed")]
        [SerializeField] private bool setSpeedParameter;
        [SerializeField] private string speedFloatParemeterName = "Speed";

        [HideInInspector][SerializeField] private Transform myTransform;
        [HideInInspector][SerializeField] private Animator myAnimator;
#pragma warning restore 0649

        private bool speedParameterHashCalculated;
        private int speedParameterHash;
        private float currentSpeed;

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.myTransform);
            this.AssertGetComponent(ref this.myAnimator);
        }

        private void Awake()
        {
            this.OnValidate();

            GPSAvatarManager.OnInitialized += () =>
            {
                GPSAvatarManager.Instance.SetAvatar(this);
            };
        }

        public Transform Transform => this.myTransform;

        public void SetSpeed(float speed)
        {
            if (this.setSpeedParameter == false)
            {
                return;
            }

            if (this.speedParameterHashCalculated == false)
            {
                this.speedParameterHash = Animator.StringToHash(this.speedFloatParemeterName);
                this.speedParameterHashCalculated = true;

                this.currentSpeed = speed;
                this.myAnimator.SetFloat(this.speedParameterHash, speed);
            }

            if (this.currentSpeed != speed)
            {
                this.currentSpeed = speed;
                this.myAnimator.SetFloat(this.speedParameterHash, speed);
            }
        }
    }
}

#endif
