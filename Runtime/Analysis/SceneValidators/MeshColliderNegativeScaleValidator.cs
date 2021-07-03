//-----------------------------------------------------------------------
// <copyright file="MeshColliderNegativeScaleValidator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MeshColliderNegativeScaleValidator : SceneValidator.Validator
    {
        public override string DisplayName => "MeshCollider Negative Scale Validator";

        public override void Run(List<SceneValidator.Error> errorResults)
        {
            foreach (var meshCollider in this.FindObjectsOfType<MeshCollider>(true))
            {
                if (this.DoesTransformHaveNegativeScale(meshCollider.transform))
                {
                    string fullPath = meshCollider.transform.GetFullPathWithSceneName();

                    errorResults.Add(new SceneValidator.Error
                    {
                        AffectedObject = meshCollider,
                        Name = $"Negative MeshCollider Scaling: {fullPath}",
                        Description = $"MeshCollider {fullPath} has negative scaling which means it's MeshCollider will be recalcuated at runtime and not precalculated during build time.",
                    });
                }
            }
        }

        private bool DoesTransformHaveNegativeScale(Transform transform)
        {
            if (transform == null)
            {
                return false;
            }
            else if (transform.localScale.x < 0 || transform.localScale.y < 0 || transform.localScale.z < 0)
            {
                return true;
            }
            else
            {
                return DoesTransformHaveNegativeScale(transform.parent);
            }
        }
    }
}

#endif
