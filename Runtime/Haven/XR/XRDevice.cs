﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="XRDevice.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum XRType
    {
        Unknown,
        ARHanheld,
        ARHeadset,
        VRHeadset,
        Pancake,
    }

    public enum XRLoader
    {
        None,
        Unknown,
        ARCore,
        ARKit,
        Oculus,
        MagicLeap,
        SteamVR,
        WindowsMixedReality,
    }

    [CreateAssetMenu(menuName = "Lost/XR Device")]
    public class XRDevice : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private XRType xrType;
        [SerializeField] private XRLoader xrLoader;
        [SerializeField] private List<string> deviceNames;
        [SerializeField] private List<string> hmdDeviceNames;
        [SerializeField] private List<RuntimePlatform> platforms;
#pragma warning restore 0649

        public XRType XRType => this.xrType;

        public XRLoader XRLoader => this.xrLoader;

        public List<string> DeviceNames => this.deviceNames;

        public List<string> HmdDeviceNames => this.hmdDeviceNames;

        public List<RuntimePlatform> Platforms => this.platforms;

        public bool IsApplicable(string deviceName, string hmdDeviceName, RuntimePlatform platform)
        {
            // Making sure platform matches
            if (this.platforms?.Count > 0 && this.platforms.Contains(platform) == false)
            {
                return false;
            }

            // Making sure device name matches
            if (this.deviceNames?.Count > 0 && this.deviceNames.Contains(deviceName) == false)
            {
                return false;
            }

            // Making sure hmd device names match
            if (this.hmdDeviceNames?.Count > 0 && this.hmdDeviceNames.Contains(hmdDeviceName) == false)
            {
                return false;
            }

            return true;
        }
    }
}

#endif
