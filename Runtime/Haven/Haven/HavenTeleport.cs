//-----------------------------------------------------------------------
// <copyright file="HavenTeleport.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace HavenXR
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [AddComponentMenu("Haven XR/Interactables/HXR Teleport")]
    public class HavenTeleport : BaseTeleportationInteractable
    {
        public enum TeleportType
        {
            Area,
            Anchor,
        }

#pragma warning disable 0649
        [SerializeField] private TeleportType type;
        [SerializeField] bool setScaleOnTeleport;
        [SerializeField] Vector3 rigScale = new Vector3(1.0f, 1.0f, 1.0f);

        [Tooltip("The Transform that represents the teleportation destination.")]
        [SerializeField] Transform teleportAnchorTransform;
#pragma warning restore 0649

        public System.Action<XRBaseInteractor, TeleportRequest> OnTeleport;

        private Transform TeleportAnchorTransform => this.teleportAnchorTransform != null ? this.teleportAnchorTransform : this.transform;

        private void OnValidate()
        {
            // Auto populating a collider if it already exists
            if (this.GetComponent<Collider>() != null && this.colliders != null && this.colliders.Count == 0)
            {
                this.colliders.Add(this.GetComponent<Collider>());
            }

            // Making sure this object is on the HavenRig Teleport Layer
            var teleportLayer = LayerMask.NameToLayer(HavenRig.TelportLayer);
            if (this.gameObject.layer != teleportLayer)
            {
                this.gameObject.layer = teleportLayer;
            }
        }

        protected void OnDrawGizmos()
        {
            if (this.type == TeleportType.Anchor)
            {
                Gizmos.color = Color.blue;
                GizmoHelpers.DrawWireCubeOriented(this.TeleportAnchorTransform.position, this.TeleportAnchorTransform.rotation, 1f);
                GizmoHelpers.DrawAxisArrows(this.TeleportAnchorTransform, 1f);
            }
        }

        protected override bool GenerateTeleportRequest(XRBaseInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (this.type == TeleportType.Area)
            {
                teleportRequest.destinationPosition = raycastHit.point;
                teleportRequest.destinationRotation = transform.rotation;
            }
            else if (this.type == TeleportType.Anchor)
            {
                teleportRequest.destinationPosition = this.TeleportAnchorTransform.position;
                teleportRequest.destinationRotation = this.TeleportAnchorTransform.rotation;
            }
            else
            {
                throw new System.NotImplementedException();
            }

            if (this.setScaleOnTeleport)
            {
                // TODO [bgish]: Get the HavenRig and set the scale
                // CoroutineRunner.Instance.ExecuteDelayed(0.1f, () => this.rigScale);
            }

            this.OnTeleport?.Invoke(interactor, teleportRequest);

            return true;
        }
    }
}

#endif
