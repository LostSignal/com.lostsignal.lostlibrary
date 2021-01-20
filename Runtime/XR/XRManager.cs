//-----------------------------------------------------------------------
// <copyright file="XRManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

#if USING_UNITY_XR_INTERACTION_TOOLKIT
    using UnityEngine.InputSystem.UI;
    using UnityEngine.UI;
    using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

    public class XRManager : Manager<XRManager>
    {
#pragma warning disable 0649
        [SerializeField] private XRDevice pancake;
        [SerializeField] private List<XRDevice> devices = new List<XRDevice>();

        [Header("Mobile AR")]
        [SerializeField] private bool setTargetFramerateOnMobileAR = true;
        [SerializeField] private int mobileArTargetFramerate = 30;

#if USING_UNITY_XR_INTERACTION_TOOLKIT
        [Header("Event Systems")]
        [SerializeField] private InputSystemUIInputModule pancakeInputSystem;
        [SerializeField] private HavenXRUIInputModule xrInputSystem;
#endif

        [Header("Debug")]
        [SerializeField] private bool printDebugInfo;
#pragma warning restore 0649

        private bool manuallyInitXRManager;

        public bool IsPancakeMode => this.CurrentDevice == this.pancake;

        public XRDevice CurrentDevice { get; private set; }

        public override void Initialize()
        {
#if !USING_UNITY_XR
            Debug.LogError("Tring to use XRManager without USING_UNITY_XR define.");
            this.SetInstance(this);
#else
            this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return CoroutineRunner.WaitForInitialization();

                if (this.printDebugInfo)
                {
                    UnityEngine.XR.XRDevice.deviceLoaded += (device) => Debug.Log($"XRManager: Device Loaded - {device}");
                    UnityEngine.XR.InputDevices.deviceConnected += (device) => Debug.Log($"XRManager: Device Connected - {device.name}");
                    UnityEngine.XR.InputDevices.deviceConfigChanged += (device) => Debug.Log($"XRManager: Device Config Changed - {device}");
                }

                if (this.printDebugInfo)
                {
                    Debug.Log($"XRManager: SystemInfo.deviceName - {SystemInfo.deviceName}");

                    // Printing off all our loaders
                    if (UnityEngine.XR.Management.XRGeneralSettings.Instance)
                    {
                        var loaders = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders;
                        Debug.Log("XRManager: XR Loader Count - " + loaders.Count);

                        for (int i = 0; i < loaders.Count; i++)
                        {
                            Debug.Log("XRManager: XR Loader - " + loaders[i]?.name ?? "NULL");
                        }
                    }
                }

                // Special case for forcing Pancake mode
                if (Application.isEditor && Application.isPlaying && ForcePancakeInEditorUtil.ForcePancakeInEditor)
                {
                    this.StartUnityXR(this.pancake.XRLoader);
                    this.FinishInitialization(this.pancake);
                    yield break;
                }

                var xrDevice = this.GetCurrentXRDevice();

                if (xrDevice != this.pancake)
                {
                    this.StartUnityXR(xrDevice.XRLoader);
                    this.FinishInitialization(xrDevice);
                }
                else
                {
                    this.StartUnityXR(this.StartLoaders());
                    this.ExecuteDelayed(1.0f, this.FinishInitialization);
                }
            }

#endif
        }

        private void FinishInitialization()
        {
            this.FinishInitialization(null);
        }

        private void FinishInitialization(XRDevice xrDevice)
        {
            if (xrDevice != null)
            {
                this.CurrentDevice = xrDevice;
            }
            else
            {
                this.CurrentDevice = this.GetCurrentXRDevice();
            }

            // Setting target framerate on mobile
            if (this.setTargetFramerateOnMobileAR && this.CurrentDevice.XRType == XRType.ARHanheld)
            {
                Application.targetFrameRate = this.mobileArTargetFramerate;
            }

#if USING_UNITY_XR_INTERACTION_TOOLKIT
            if (this.IsPancakeMode)
            {
                this.pancakeInputSystem.enabled = true;
                this.xrInputSystem.enabled = false;
            }
            else
            {
                this.pancakeInputSystem.enabled = false;
                this.xrInputSystem.enabled = true;
            }
#endif

            if (this.printDebugInfo)
            {
                Debug.Log($"Current Device = {this.CurrentDevice.name}");
            }

            if (this.IsPancakeMode == false)
            {
                //// TODO [bgish]: Uncomment this out when keyboard is ready for prime time
                //// this.ListenForXRKeyboard();
            }

            this.SetInstance(this);
            this.ListenForXRKeyboard();
        }

        private void StartUnityXR(XRLoader xrLoader)
        {
#if USING_UNITY_XR
            // Starting up the XR Manager if needed
            if (xrLoader != XRLoader.None && xrLoader != XRLoader.Unknown && UnityEngine.XR.Management.XRGeneralSettings.Instance.InitManagerOnStart == false)
            {
                MoveLoaderToTop(xrLoader);

                this.manuallyInitXRManager = true;
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
            }

            void MoveLoaderToTop(XRLoader loader)
            {
                string loaderName = this.GetLoaderName(loader);

                if (loaderName == null)
                {
                    Debug.LogError($"Unkonwn XRLoader Type \"{xrLoader}\" encountered");
                    return;
                }

                for (int i = 0; i < UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders.Count; i++)
                {
                    var unityXRLoader = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders[i];

                    if (unityXRLoader?.name == loaderName)
                    {
                        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders.RemoveAt(i);
                        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders.Insert(0, unityXRLoader);
                        return;
                    }
                }

                Debug.LogError($"Unable to find XR Loader in the XR Settings {loaderName}");
            }

#endif
        }

        private string GetLoaderName(XRLoader loader)
        {
            if (loader == XRLoader.None || loader == XRLoader.Unknown)
            {
                return null;
            }
            else if (loader == XRLoader.Oculus)
            {
                return "Oculus Loader";
            }
            else if (loader == XRLoader.ARCore)
            {
                return "AR Core Loader";
            }
            else if (loader == XRLoader.ARKit)
            {
                return "AR Kit Loader";
            }
            else if (loader == XRLoader.MagicLeap)
            {
                return "Magic Leap Loader";
            }
            else if (loader == XRLoader.WindowsMixedReality)
            {
                return "Windows MR Loader";
            }
            else if (loader == XRLoader.SteamVR)
            {
                return null; // "Steam VR Loader";
            }
            else
            {
                Debug.LogError($"Found Unknown XRLoader {loader}");
                return null;
            }
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

        private XRLoader StartLoaders()
        {
#if USING_UNITY_XR
            var xrLoaders = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.loaders;

            foreach (var loader in Enum.GetValues(typeof(XRLoader)))
            {
                XRLoader xrLoader = (XRLoader)loader;

                if (xrLoader == XRLoader.None || xrLoader == XRLoader.Unknown)
                {
                    continue;
                }

                var loaderName = this.GetLoaderName(xrLoader);

                if (loaderName == null)
                {
                    continue;
                }

                for (int i = 0; i < xrLoaders.Count; i++)
                {
                    try
                    {
                        if (xrLoaders[i]?.name == loaderName)
                        {
                            if (xrLoaders[i].Start())
                            {
                                return xrLoader;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
#endif

            return XRLoader.None;
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

                if (this.printDebugInfo)
                {
                    Debug.Log($"Head Device = {hmdDevice.name}, Presence Supported = {presenceSupported}, User Present = {userPresent}");
                }

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

        private void ListenForXRKeyboard()
        {
            this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                XRKeyboard xrKeyboard = DialogManager.GetDialog<XRKeyboard>();

                while (true)
                {
                    if (xrKeyboard.Dialog.IsHidden && (InputFieldTracker.GetCurrentInputField() != null || InputFieldTracker.GetCurrentTMPInputField() != null))
                    {
                        xrKeyboard.Dialog.Show();
                    }

                    yield return WaitForUtil.Seconds(0.25f);
                }
            }
        }
    }
}
