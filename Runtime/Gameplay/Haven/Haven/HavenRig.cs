//-----------------------------------------------------------------------
// <copyright file="HavenRig.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using Lost;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.XR.Interaction.Toolkit;

    public class HavenRig : MonoBehaviour
    {
        public const string TelportLayer = "Teleport";

#pragma warning disable 0649
        [Header("Rigs")]
        [SerializeField] private XRRig genericXRRig;
        [SerializeField] private Camera rigCamera;
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;
        [SerializeField] private PancakeController pancakeController;
#pragma warning restore 0649

        public static IEnumerator WaitForRig()
        {
            while (true)
            {
                if (ObjectTracker.Instance && ObjectTracker.Instance.GetFirstObject<HavenRig>() != null)
                {
                    yield break;
                }

                yield return null;
            }
        }

        public static HavenRig GetRig()
        {
            if (ObjectTracker.Instance)
            {
                return ObjectTracker.Instance.GetFirstObject<HavenRig>();
            }

            return null;
        }

        public Camera RigCamera => this.rigCamera;

        public Transform LeftController => this.leftController;

        public Transform RightController => this.rightController;

        public float RigScale => this.genericXRRig.transform.localScale.x;

        public void SetScale(float scale)
        {
            this.genericXRRig.transform.localScale = new Vector3(scale, scale, scale);
        }

        private void Awake()
        {
            XRManager.OnInitialized += this.Initialize;

            //// TODO [bgish]: Make sure all the Teleport Interactors have thier interaction layer set to HavenRig.TeleportLayer
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

        private void Initialize()
        {
            var device = XRManager.Instance.CurrentDevice;

            if (device.XRType == XRType.VRHeadset)
            {
                // Do Nothing
            }
            else if (device.XRType == XRType.Pancake)
            {
                this.pancakeController.enabled = true;

                // For some reason Pancake doesn't properly set the camera Y offset, so doing it manually
                this.genericXRRig.cameraFloorOffsetObject.transform.localPosition =
                    this.genericXRRig.cameraFloorOffsetObject.transform.localPosition.SetY(this.genericXRRig.cameraYOffset);
            }
            else if (device.XRType == XRType.ARHanheld)
            {
                // ...
            }
            else if (device.XRType == XRType.ARHeadset)
            {
                // ...
            }
            else
            {
                throw new System.NotImplementedException();
            }

        }
    }
}

#endif
