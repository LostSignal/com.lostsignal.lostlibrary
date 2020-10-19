//-----------------------------------------------------------------------
// <copyright file="XRDialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    // Right now the PancakeDevice.Awake (line 47) sets "Cursor.lockState = CursorLockMode.Locked;"
    // Should this be a part of the XRManager
    // Whenever a Dialog is shown that has "RequireMouseInPancakeMode", it will turn the mouse back on?????

    // XR Manager
    //   Cursor / Mouse Icon

    // XR Settings
    //     bool DoesFollowUser
    //         float canvasDistanceForwards;
    //         float canvasDistanceUpwards = 0.0f;
    //         float positionLerpSpeed = 2.0f;
    //         float rotationLerpSpeed = 10.0f;
    //
    //     bool isManipulableByUser;
    //         float min/max distance;
    //
    //     bool requiresMouseInPancakeMode;
    //
    //     bool staticWorldSpace;


    [RequireComponent(typeof(Dialog))]
    public class XRDialog : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private XRDialogSettings settings;
        [SerializeField] private bool isXrMode;
#pragma warning restore 0649

        private Dialog dialog;
        private Canvas dialogCanvas;
        private float originalPlaneDistance;

        private bool IsXRApplication
        {
            get => XRManager.Instance != null ? XRManager.Instance.enabled : false;
        }

        private bool IsPancakeMode
        {
            get => XRManager.Instance != null ? XRManager.Instance.IsPancakeMode : true;
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.dialog);
            this.AssertGetComponent(ref this.dialogCanvas);
        }

        private void Awake()
        {
            this.OnValidate();
            this.enabled = this.dialog.ShowOnAwake;
            this.dialog.OnShow.AddListener(this.OnShow);
            this.originalPlaneDistance = this.dialogCanvas.planeDistance;
        }

        private void Update()
        {
            var dialogCamera = this.dialogCanvas.worldCamera;

            if (dialogCamera == null)
            {
                return;
            }

            var canvasSettings = this.settings.CurrentSettings;
            var dialogCameraTransform = dialogCamera.transform;

            // Move the object CanvasDistance units in front of the camera
            float positionSpeed = Time.deltaTime * canvasSettings.PositionLerpSpeed;

            Vector3 positionTo =
                dialogCameraTransform.position +
                (dialogCameraTransform.forward * canvasSettings.CanvasDistanceForwards) +
                (dialogCameraTransform.up * canvasSettings.CanvasDistanceUpwards);

            this.transform.position = Vector3.SlerpUnclamped(this.transform.position, positionTo, positionSpeed);

            // Rotate the object to face the camera
            float rotattionSpeed = Time.deltaTime * canvasSettings.RotationLerpSpeed;
            Vector3 diff = this.transform.position - dialogCameraTransform.position;

            if (diff != Vector3.zero)
            {
                Quaternion rotationTo = Quaternion.LookRotation(diff);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationTo, rotattionSpeed);
            }
        }

        private void OnShow()
        {
            if (this.IsXRApplication && this.IsPancakeMode == false)
            {
                this.enabled = true;

                if (this.dialogCanvas.renderMode != RenderMode.WorldSpace)
                {
                    this.dialogCanvas.planeDistance = this.settings.CurrentSettings.CanvasDistanceForwards;

                    this.ExecuteAtEndOfFrame(() =>
                    {
                        this.dialogCanvas.renderMode = RenderMode.WorldSpace;

                        // Making sure the dialog is directly in front of the camera at the settings canvas distance
                        this.dialogCanvas.transform.position =
                            this.dialogCanvas.worldCamera.transform.position +
                            (this.dialogCanvas.worldCamera.transform.forward.normalized * this.settings.CurrentSettings.CanvasDistanceForwards);
                    });
                }
            }
            else
            {
                this.enabled = false;

                if (this.dialogCanvas.renderMode == RenderMode.WorldSpace)
                {
                    this.dialogCanvas.renderMode = this.dialog.IsOverlayCamera ? RenderMode.ScreenSpaceOverlay : RenderMode.ScreenSpaceCamera;
                    this.dialogCanvas.planeDistance = this.originalPlaneDistance;
                }
            }
        }
    }
}
