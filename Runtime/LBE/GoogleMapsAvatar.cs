//-----------------------------------------------------------------------
// <copyright file="GoogleMapsAvatar.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_GOOGLE_MAPS_SDK && USING_CINEMACHINE

namespace Lost
{
    using Cinemachine;
    using Lost.LBE;
    using System.Collections.Generic;
    using UnityEngine;

    ////
    //// To Do:
    ////  * Update Avatar to use Cinemachine Camera System
    ////  * Add Rotate (Pan Side to side / Right Mouse side to side)
    ////  * Add Zoom (Pinch / Scroll Wheel)
    ////
    public class GoogleMapsAvatar : MonoBehaviour, InputHandler
    {
        #pragma warning disable 0649
        [Header("Avatar")]
        [SerializeField] private GameObject avatarBody;
        [SerializeField] private float avatarRoationSpeed = 1.0f;
        [SerializeField] private AnimationCurve zoomToScaleCurve;

        [Header("Camera")]
        [SerializeField] private CinemachineMixingCamera avatarCamera;
        [SerializeField] private CinemachineVirtualCamera zoomedInCamera;
        [SerializeField] private CinemachineVirtualCamera zoomedOutCamera;
        
        [Header("Rotation / Zoom")]
        [SerializeField] private float rotation;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float zoom;

        [Header("Rotation / Zoom Sensitivity")]
        [SerializeField] private float rotationSensitivity = 100.0f;
        #pragma warning restore 0649

        private CinemachineOrbitalTransposer zoomedInOrbitalTransposer;
        private CinemachineOrbitalTransposer zoomedOutOrbitalTransposer;
        private bool isBodyVisible;

        private void Awake()
        {
            this.avatarBody.SetActive(false);
            this.isBodyVisible = false;

            this.zoom = 0.5f; // Keep track of the players perferred zoom level and restore it here

            this.zoomedInOrbitalTransposer = this.zoomedInCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            this.zoomedOutOrbitalTransposer = this.zoomedOutCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

            InputManager.OnInitialized += InitializeInputManager;

            void InitializeInputManager()
            {
                InputManager.Instance.AddHandler(this);
            }
        }

        private void Update()
        {
            if (GPSManager.IsInitialized == false || GoogleMapsManager.IsInitialized == false || GoogleMapsManager.Instance.IsMapLoaded == false)
            {
                return;
            }

            if (this.isBodyVisible != GoogleMapsManager.Instance.IsMapLoaded)
            {
                this.isBodyVisible = GoogleMapsManager.Instance.IsMapLoaded;
                this.avatarBody.SetActive(this.isBodyVisible);
            }

            // TODO [bgish]: Eventually listen for GPS data to figure out which way to point the avatar (if phone has magnatrometer, then use that instead)
        }

        void InputHandler.HandleInputs(List<Input> touches, Input mouse, Input pen)
        {
            if (Application.isEditor && mouse.InputState == InputState.Moved)
            {
                this.ProcessRotate(mouse);
            }
            else if (touches.Count == 1)
            {
                this.ProcessRotate(touches[0]);
            }
            else if (touches.Count > 1)
            {
                this.ProcessZoom(touches[0], touches[1]);
            }
        }

        private void ProcessRotate(Input input)
        {   
            float xDelta = input.CurrentPosition.x - input.PreviousPosition.x;
            float xDeltaPercentOfScreen = xDelta / Screen.width;

            this.rotation += xDeltaPercentOfScreen * this.rotationSensitivity;
        }

        private void ProcessZoom(Input input1, Input input2)
        {
            // TODO [bgish]: Implement
        }
    }
}

#endif
