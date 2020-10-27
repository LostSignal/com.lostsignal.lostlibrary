//-----------------------------------------------------------------------
// <copyright file="XRManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class XRManager : Manager<XRManager>
    {
#pragma warning disable 0649
        [SerializeField] private XRDevice pancake;
        [SerializeField] private List<XRDevice> devices = new List<XRDevice>();
#pragma warning restore 0649

        private bool manuallyInitXRManager;
        private XRDevice currentDevice;

        public bool IsPancakeMode => this.CurrentDevice == this.pancake;

        public XRDevice CurrentDevice
        {
            get
            {
                if (this.currentDevice == null)
                {
                    this.currentDevice = this.GetCurrentXRDevice();
                }

                return this.currentDevice;
            }
        }

        public override void Initialize()
        {
#if !USING_UNITY_XR
            Debug.LogError("Tring to use XRManager without USING_UNITY_XR define.");
            this.SetInstance(this);
#else

            UnityEngine.XR.XRDevice.deviceLoaded += (device) =>
            {
                Debug.Log("Device Loaded: " + device);
            };

            UnityEngine.XR.InputDevices.deviceConnected += (device) =>
            {
                Debug.Log("Device Connected: " + device.name);
            };

            UnityEngine.XR.InputDevices.deviceConfigChanged += (device) =>
            {
                Debug.Log("Device Config Changed: " + device.ToString());
            };

            Debug.Log($"SystemInfo.deviceName = {SystemInfo.deviceName}");

            // Printing off all our loaders
            if (UnityEngine.XR.Management.XRGeneralSettings.Instance)
            {
                var loaders = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders;
                Debug.Log("XR Loader Count: " + loaders.Count);

                for (int i = 0; i < loaders.Count; i++)
                {
                    Debug.Log("XR Loader: " + loaders[i]?.name ?? "NULL");
                }
            }

            if (this.CurrentDevice == null)
            {
                Debug.LogError("Found Unknown XR Device");
                Platform.QuitApplication();
                return;
            }

            // AR mobile apps work great at 30 fps, so lets throttle to that to save battery
            if (this.CurrentDevice.XRType == XRType.ARHanheld)
            {
                Application.targetFrameRate = 30;
            }

            UpdateLoader(this.CurrentDevice.XRLoader);

            // Starting up the XR Manager if needed
            if (this.CurrentDevice.XRLoader != XRLoader.None && UnityEngine.XR.Management.XRGeneralSettings.Instance.InitManagerOnStart == false)
            {
                this.manuallyInitXRManager = true;
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
            }

            this.SetInstance(this);

            void UpdateLoader(XRLoader loader)
            {
                // Making sure the correct loader is at the top
                if (loader == XRLoader.Oculus)
                {
                    MoveLoaderToTop("Oculus Loader");
                }
                else if (loader == XRLoader.ARCore)
                {
                    MoveLoaderToTop("AR Core Loader");
                }
                else if (loader == XRLoader.ARKit)
                {
                    MoveLoaderToTop("AR Kit Loader");
                }
                else if (loader == XRLoader.MagicLeap)
                {
                    MoveLoaderToTop("Magic Leap Loader");
                }
                else if (loader == XRLoader.WindowsMixedReality)
                {
                    MoveLoaderToTop("Windows MR Loader");
                }
                else if (loader == XRLoader.SteamVR)
                {
                    // MoveLoaderToTop("Steam VR Loader");
                    throw new System.NotImplementedException();
                }
                else if (loader == XRLoader.None)
                {
                    // Do Nothing
                }
                else
                {
                    Debug.LogError($"Unkonwn XRLoader Type \"{loader}\" encountered");
                }
            }

            void MoveLoaderToTop(string loaderName)
            {
                for (int i = 0; i < UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders.Count; i++)
                {
                    var loader = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders[i];

                    if (loader?.name == loaderName)
                    {
                        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders.RemoveAt(i);
                        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders.Insert(0, loader);
                        return;
                    }
                }

                Debug.LogError($"Unable to find XR Loader {loaderName}");
            }

#endif
        }

        private void OnDestroy()
        {
#if USING_UNITY_XR
            if (this.manuallyInitXRManager)
            {
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
#endif
        }

        private XRDevice GetCurrentXRDevice()
        {
#if USING_UNITY_XR
            if (Application.isEditor && Application.isPlaying && ForcePancakeInEditorUtil.ForcePancakeInEditor)
            {
                return this.pancake;
            }

            var platform = Application.platform;
            var deviceName = SystemInfo.deviceName;

            // Calculating the currently connected HMD
            var hmdDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head);
            var hmdDeviceName = string.Empty;

            if (hmdDevice.isValid)
            {
                bool presenceSupported = hmdDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.userPresence, out bool userPresent);
                Debug.Log($"Head Device = {hmdDevice.name}, Presence Supported = {presenceSupported}, User Present = {userPresent}");

                if (presenceSupported)
                {
                    hmdDeviceName = hmdDevice.name;
                }
            }

            foreach (var device in this.devices)
            {
                if (device.IsApplicable(deviceName, hmdDeviceName, platform))
                {
                    return device;
                }
            }
#endif

            return this.pancake;
        }
    }
}
