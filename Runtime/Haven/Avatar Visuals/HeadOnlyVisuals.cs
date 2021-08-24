//-----------------------------------------------------------------------
// <copyright file="HeadOnlyVisuals.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using Lost.Networking;
    using UnityEngine;

    public class HeadOnlyVisuals : HavenAvatarVisuals
    {
#pragma warning disable 0649
        [Header("Head Transform")]
        [SerializeField] private Transform head;
#pragma warning restore 0649

        private HavenRig rig;

        private bool desiredValuesSet;
        private Vector3 desiredHeadLocalPosition;
        private Quaternion desiredHeadLocalRotation;

        private void Start()
        {
            if (this.IsOwner)
            {
                this.ShowVisuals(false);
                this.rig = HavenRig.GetRig();
            }
        }

        private void LateUpdate()
        {
            if (this.desiredValuesSet)
            {
                this.head.localPosition = Vector3.Lerp(this.head.localPosition, this.desiredHeadLocalPosition, Time.deltaTime);
                this.head.localRotation = Quaternion.Slerp(this.head.localRotation, this.desiredHeadLocalRotation, 5.0f);
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.rig.RigCamera.transform.localPosition);
            writer.Write(this.rig.RigCamera.transform.localRotation);
        }

        public override void Deserialize(NetworkReader reader)
        {
            this.desiredValuesSet = true;
            this.desiredHeadLocalPosition = reader.ReadVector3();
            this.desiredHeadLocalRotation = reader.ReadQuaternion();
        }
    }
}

#endif
