//-----------------------------------------------------------------------
// <copyright file="AttachToMainCamera.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public class AttachToMainCamera : MonoBehaviour
    {
        private Camera mainCamera;

        private void LateUpdate()
        {
            if (!this.mainCamera)
            {
                this.mainCamera = Camera.main;
            }

            if (this.mainCamera)
            {
                this.transform.SetPositionAndRotation(this.mainCamera.transform.position, this.mainCamera.transform.rotation);
            }
        }
    }
}

#endif
