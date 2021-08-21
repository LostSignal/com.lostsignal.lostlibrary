#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="CameraFacingBillboard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

// http://wiki.unity3d.com/index.php?title=CameraFacingBillboard&_ga=2.46806503.1571963746.1612411656-2131485283.1592860438

namespace Lost
{
    using UnityEngine;

    public class CameraFacingBillboard : MonoBehaviour
    {
        private Camera mainCamera;

        private void LateUpdate()
        {
            if (this.mainCamera == null)
            {
                this.mainCamera = Camera.main;
            }

            if (this.mainCamera != null)
            {
                this.transform.LookAt(this.mainCamera.transform);
            }
        }
    }
}

#endif
