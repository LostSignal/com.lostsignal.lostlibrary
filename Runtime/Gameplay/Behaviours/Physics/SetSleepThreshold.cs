//-----------------------------------------------------------------------
// <copyright file="SetSleepThreshold.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class SetSleepThreshold : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField, HideInInspector] private Rigidbody rigidBody;
        [SerializeField] private float sleepThreshold = 0.00001f;
        #pragma warning restore 0649

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.rigidBody);
        }

        private void Awake()
        {
            this.OnValidate();
            this.rigidBody.sleepThreshold = this.sleepThreshold;
        }
    }
}

#endif
