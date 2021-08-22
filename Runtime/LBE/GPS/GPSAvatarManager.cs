//-----------------------------------------------------------------------
// <copyright file="GPSAvatarManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_CINEMACHINE

namespace Lost
{
    using System.Collections;
    using System.Collections.Generic;
    using Cinemachine;
    using UnityEngine;

    ////
    //// To Do:
    ////  * Update Avatar to use Cinemachine Camera System
    ////  * Add Rotate (Pan Side to side / Right Mouse side to side)
    ////  * Add Zoom (Pinch / Scroll Wheel)
    ////
    public class GPSAvatarManager :
        Manager<GPSAvatarManager>,
        InputHandler
    {
        #pragma warning disable 0649
        [Header("Avatar Scaling")]
        [SerializeField] private AnimationCurve zoomToScaleCurve;

        [Header("Camera")]
        [SerializeField] private CinemachineMixingCamera avatarCamera;
        [SerializeField] private CinemachineVirtualCamera zoomedInCamera;
        [SerializeField] private CinemachineVirtualCamera zoomedOutCamera;

        [Header("Camera Rotation / Zoom")]
        [SerializeField] private float rotation;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float zoom;

        [Header("Rotation / Zoom Sensitivity")]
        [SerializeField] private float rotationSensitivity = 100.0f;
        [SerializeField] private float zoomInputScale = 0.3f;
        #pragma warning restore 0649

        private CinemachineOrbitalTransposer zoomedInOrbitalTransposer;
        private CinemachineOrbitalTransposer zoomedOutOrbitalTransposer;
        private GPSAvatar gpsAvatar;

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        public void SetAvatar(GPSAvatar avatar)
        {
            this.gpsAvatar = avatar;
        }

        protected override void Awake()
        {
            base.Awake();

            this.zoom = 0.5f; // TODO [bgish]: Use PlayerData and keep track of the players last zoom level and restore it here
            this.zoomedInOrbitalTransposer = this.zoomedInCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            this.zoomedOutOrbitalTransposer = this.zoomedOutCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

            InputManager.OnInitialized += () =>
            {
                InputManager.Instance.AddHandler(this);
            };
        }

        private IEnumerator Start()
        {
            // Waiting for all the managers to finsih initializing before updating teh avatar
            while (GPSManager.IsInitialized == false || GPSDirectionManager.IsInitialized == false || GPSPositionManager.IsInitialized == false || GPSManager.Instance.HasReceivedGpsData == false)
            {
                yield return null;
            }

            while (true)
            {
                // Updating camera weights based on zoom level
                float clampedZoom = Mathf.Clamp01(this.zoom);
                this.avatarCamera.SetWeight(0, 1.0f - clampedZoom);
                this.avatarCamera.SetWeight(1, clampedZoom);

                // Updating Camera rotation around the Avatar
                this.zoomedInOrbitalTransposer.m_XAxis.Value = this.rotation;
                this.zoomedOutOrbitalTransposer.m_XAxis.Value = this.rotation;

                this.UpdateAvatar();

                yield return null;
            }
        }

        private void UpdateAvatar()
        {
            if (this.gpsAvatar == null)
            {
                return;
            }

            // Updating Avatar Body Scale
            float scale = this.zoomToScaleCurve.Evaluate(this.zoom);

            if (this.gpsAvatar.Transform.localScale.x != scale)
            {
                this.gpsAvatar.Transform.localScale = new Vector3(scale, scale, scale);
            }

            // Updating Avatar Body Rotation
            var gpsDirection = GPSDirectionManager.Instance.Direction;

            if (this.gpsAvatar.Transform.localRotation != gpsDirection)
            {
                this.gpsAvatar.Transform.localRotation = gpsDirection;
            }

            // Updating Speed
            this.gpsAvatar.SetSpeed(GPSPositionManager.Instance.Speed);
        }

        void InputHandler.HandleInputs(List<Input> touches, Input mouse, Input pen)
        {
            if (Application.isEditor)
            {
                if (mouse.InputState == InputState.Moved)
                {
                    #if USING_UNITY_INPUT_SYSTEM
                    bool isLeftCtrlDown = UnityEngine.InputSystem.Keyboard.current.leftCtrlKey.isPressed;
                    #else
                    bool isLeftCtrlDown = UnityEngine.Input.GetKeyDown(KeyCode.LeftControl);
                    #endif

                    if (isLeftCtrlDown == false)
                    {
                        this.ProcessRotate(mouse);
                    }
                    else
                    {
                        // Creating a fake input in the center of the screen
                        Input fakeInput = new Input();
                        fakeInput.Reset(0, 0, InputType.Mouse, InputButton.Left, new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));

                        this.ProcessZoom(mouse, fakeInput);
                    }
                }
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
            float zoomFactor = Input.CalculatePinchZoomFactor(input1, input2, out _);
            this.zoom = Mathf.Clamp01(this.zoom - (zoomFactor * this.zoomInputScale));
        }
    }
}

#endif
