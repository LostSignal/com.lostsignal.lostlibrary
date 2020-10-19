//-----------------------------------------------------------------------
// <copyright file="ScreenSizeManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public class ScreenSizeManager : Manager<ScreenSizeManager>
    {
        private const int MaxScreenSize = 1920;

#pragma warning disable 0649
        [SerializeField] private bool limitMobileScreenSizeTo1080p = true;

        [Header("Dependencies")]
        [SerializeField] private XRManager xrManager;
#pragma warning restore 0649

        public override void Initialize()
        {
            this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                bool isMobilePlatform = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
                bool isMobileVrDevice = false;

                // Detecting if this is a mobile vr device
                if (this.xrManager != null && this.xrManager.enabled)
                {
                    yield return this.WaitForDependencies(this.xrManager);
                    isMobilePlatform = this.xrManager.CurrentDevice.XRType == XRType.VRHeadset;
                }

                if (this.limitMobileScreenSizeTo1080p && isMobilePlatform && isMobileVrDevice == false)
                {
                    bool isLandscape = Screen.width > Screen.height;

                    if (isLandscape && Screen.width > MaxScreenSize)
                    {
                        float aspectRatio = Screen.height / (float)Screen.width;
                        int newHeight = (int)(MaxScreenSize * aspectRatio);
                        int newWidth = MaxScreenSize;

                        Debug.LogFormat("Resizing Screen From {0}x{1} To {2}x{3}", Screen.width, Screen.height, newWidth, newHeight);
                        Screen.SetResolution(newWidth, newHeight, true);
                    }
                    else if (isLandscape == false && Screen.height > MaxScreenSize)
                    {
                        float aspectRatio = Screen.width / (float)Screen.height;
                        int newHeight = MaxScreenSize;
                        int newWidth = (int)(MaxScreenSize * aspectRatio);

                        Debug.LogFormat("Resizing Screen From {0}x{1} To {2}x{3}", Screen.width, Screen.height, newWidth, newHeight);
                        Screen.SetResolution(newWidth, newHeight, true);
                    }
                }

                this.SetInstance(this);
            }
        }
    }
}
