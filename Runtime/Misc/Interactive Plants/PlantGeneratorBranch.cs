//-----------------------------------------------------------------------
// <copyright file="PlantGeneratorBranch.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.PlantGenerator
{
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// This class represents a branch of the plant generator.  You setup a mesh with an aim target and this
    /// class will make sure the mesh points towards that aim target.  In order for everything to work
    /// properly, the branch should be facing down the forward vector (0, 0, 1).
    /// </summary>
    public class PlantGeneratorBranch : MonoBehaviour
    {
#pragma warning disable 0649
        [Tooltip("How fast the MeshRoot should lerp towards the mesh aim target.")]
        [FormerlySerializedAs("LerpSpeed")]
        [SerializeField] private float lerpSpeed = 5;

        [Tooltip("The transform that the MeshRoot should always point at.")]
        [FormerlySerializedAs("AimTarget")]
        [SerializeField] private Transform aimTarget;

        [Tooltip("The Mesh's parent that will be rotated to make sure this mesh always points to the MeshAimTarget.")]
        [FormerlySerializedAs("Mesh")]
        [SerializeField] private Transform mesh;
#pragma warning restore 0649

        private Rigidbody aimTargetRigidbody;

        private void Awake()
        {
            this.aimTargetRigidbody = this.aimTarget.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Updates the MeshRoot rotation point towards the mesh aim target.
        /// </summary>
        private void Update()
        {
            if (this.aimTargetRigidbody.IsSleeping() == false)
            {
                this.mesh.rotation = Quaternion.Lerp(this.mesh.rotation, this.aimTarget.rotation, this.lerpSpeed * Time.deltaTime);
            }
        }
    }
}

#endif
