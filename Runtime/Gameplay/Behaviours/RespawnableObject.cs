//-----------------------------------------------------------------------
// <copyright file="RespawnableObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [AddComponentMenu("Lost/Respawnable Object")]
    public class RespawnableObject : MonoBehaviour
    {
        private Vector3 startingPosition;
        private Quaternion startingRotation;
        private Rigidbody objectsRigidbody;

        public void RespawnToStartPosition()
        {
            if (this.objectsRigidbody == null)
            {
                this.transform.position = this.startingPosition;
                this.transform.rotation = this.startingRotation;
            }
            else
            {
                this.objectsRigidbody.velocity = Vector3.zero;
                this.objectsRigidbody.angularVelocity = Vector3.zero;
                this.objectsRigidbody.position = this.startingPosition;
                this.objectsRigidbody.rotation = this.startingRotation;
            }
        }

        public void RespawnToTransform(Transform respawnTransform)
        {
            if (this.objectsRigidbody == null)
            {
                this.transform.position = respawnTransform.transform.position;
                this.transform.rotation = respawnTransform.transform.rotation;
            }
            else
            {
                this.objectsRigidbody.velocity = Vector3.zero;
                this.objectsRigidbody.angularVelocity = Vector3.zero;
                this.objectsRigidbody.position = respawnTransform.transform.position;
                this.objectsRigidbody.rotation = respawnTransform.transform.rotation;
            }
        }

        private void Start()
        {
            this.objectsRigidbody = this.GetComponent<Rigidbody>();

            if (this.objectsRigidbody == null)
            {
                this.startingPosition = this.transform.position;
                this.startingRotation = this.transform.rotation;
            }
            else
            {
                this.startingPosition = this.objectsRigidbody.position;
                this.startingRotation = this.objectsRigidbody.rotation;
            }
        }
    }
}
