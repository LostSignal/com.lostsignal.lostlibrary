//-----------------------------------------------------------------------
// <copyright file="ApplyImpulse.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class ApplyImpulse : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] [HideInInspector] private Rigidbody rigidBody;
        [SerializeField] private float impulseForce = 1.0f;
        #pragma warning restore 0649

        [CalledByUnityEvent]
        public void Apply(RaycastHit hit)
        {
            Vector3 force = hit.normal.normalized * -this.impulseForce;
            this.rigidBody.AddForceAtPosition(force, hit.point);
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.rigidBody);
        }

        private void Awake()
        {
            this.OnValidate();
        }
    }
}

#endif
