//-----------------------------------------------------------------------
// <copyright file="NetworkTransformAnchor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Networking
{
    using UnityEngine;

    public class NetworkTransformAnchor : MonoBehaviour
    {
        public static NetworkTransformAnchor GetAnchor()
        {
            return ObjectTracker.Instance.GetFirstObject<NetworkTransformAnchor>();
        }

        public static Vector3 InverseTransformPosition(Vector3 worldSpace)
        {
            var anchor = GetAnchor();
            return anchor ? anchor.transform.InverseTransformPoint(worldSpace) : worldSpace;
        }

        public static Quaternion InverseTransformRotation(Quaternion worldRotation)
        {
            var anchor = GetAnchor();
            return anchor ? Quaternion.Inverse(anchor.transform.rotation) * worldRotation : worldRotation;
        }

        public static Vector3 TransformPosition(Vector3 anchorSpacePosition)
        {
            var anchor = GetAnchor();
            return anchor ? anchor.transform.TransformPoint(anchorSpacePosition) : anchorSpacePosition;
        }

        public static Quaternion TransformRotation(Quaternion anchorSpaceRotation)
        {
            var anchor = GetAnchor();
            return anchor ? anchor.transform.rotation * anchorSpaceRotation : anchorSpaceRotation;
        }

        private void OnEnable()
        {
            Bootloader.OnBoot += this.UpdateObjectTracker;
        }

        private void OnDisable()
        {
            Bootloader.OnBoot -= this.UpdateObjectTracker;
            this.UpdateObjectTracker();
        }

        private void UpdateObjectTracker()
        {
            ObjectTracker.UpdateRegistration(this);
        }
    }
}

#endif
