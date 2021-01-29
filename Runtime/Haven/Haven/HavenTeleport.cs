//-----------------------------------------------------------------------
// <copyright file="HavenTeleport.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
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
        [SerializeField] private bool setScaleOnTeleport;
        [SerializeField] private float rigScale = 1.0f;

        [Tooltip("The Transform that represents the teleportation destination.")]
        [SerializeField] private Transform anchorOverrideTransform;
#pragma warning restore 0649

        public System.Action<XRBaseInteractor, TeleportRequest> OnTeleport;

        private Transform AnchorOverrideTransform => this.anchorOverrideTransform != null ? this.anchorOverrideTransform : this.transform;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (this.interactionManager != null)
            {
                this.interactionManager = null;
            }

            if (this.interactionLayerMask != LayerMask.GetMask(HavenRig.TelportLayer))
            {
                this.interactionLayerMask = LayerMask.GetMask(HavenRig.TelportLayer);
            }

            if (this.teleportTrigger != TeleportTrigger.OnSelectExited)
            {
                this.teleportTrigger = TeleportTrigger.OnSelectExited;
            }

            if (this.teleportationProvider != null)
            {
                this.teleportationProvider = null;
            }

            // Making sure this object is on the HavenRig Teleport Layer
            this.gameObject.SetLayerRecursively(LayerMask.NameToLayer(HavenRig.TelportLayer));

            // Auto populating a collider if it already exists
            var colliders = this.GetComponentsInChildren<Collider>();

            if (colliders?.Length > 0 && this.colliders != null && this.colliders.Count == 0)
            {
                foreach (var collider in colliders)
                {
                    this.colliders.Add(collider);
                }
            }
        }

        protected void OnDrawGizmos()
        {
            if (this.type == TeleportType.Anchor)
            {
                Gizmos.color = Color.blue;
                GizmoHelpers.DrawWireCubeOriented(this.AnchorOverrideTransform.position, this.AnchorOverrideTransform.rotation, 1f);
                GizmoHelpers.DrawAxisArrows(this.AnchorOverrideTransform, 1f);
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
                teleportRequest.destinationPosition = this.AnchorOverrideTransform.position;
                teleportRequest.destinationRotation = this.AnchorOverrideTransform.rotation;
            }
            else
            {
                throw new System.NotImplementedException();
            }

            if (this.setScaleOnTeleport)
            {
                CoroutineRunner.Instance.ExecuteDelayed(0.1f, () => HavenRig.GetRig().SetScale(this.rigScale));
            }

            this.OnTeleport?.Invoke(interactor, teleportRequest);

            return true;
        }
    }
}

#endif
