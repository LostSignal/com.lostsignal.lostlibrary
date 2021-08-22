//-----------------------------------------------------------------------
// <copyright file="ScreenSizeManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections;
    using UnityEngine;

    public sealed class ScreenSizeManager : Manager<ScreenSizeManager>
    {
        public override void Initialize()
        {
            this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return ReleasesManager.WaitForInitialization();

                var settings = ReleasesManager.Instance.CurrentRelease.ScreenSizeManagerSettings;

                if (settings.LimitMobileScreenSize)
                {
                    yield return this.LimitScreenSize(settings.MaxScreenSize);
                }

                this.SetInstance(this);
            }
        }

        private IEnumerator LimitScreenSize(int maxScreenSize)
        {
            bool isMobilePlatform = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            bool isMobileVrDevice = false;

            // Detecting if this is a mobile vr device
            if (ReleasesManager.Instance.CurrentRelease.IsXRApp)
            {
                yield return XRManager.WaitForInitialization();
                isMobilePlatform = XRManager.Instance.CurrentDevice.XRType == XRType.VRHeadset;
            }

            if (isMobilePlatform && isMobileVrDevice == false)
            {
                bool isLandscape = Screen.width > Screen.height;

                if (isLandscape && Screen.width > maxScreenSize)
                {
                    float aspectRatio = Screen.height / (float)Screen.width;
                    int newHeight = (int)(maxScreenSize * aspectRatio);
                    int newWidth = maxScreenSize;

                    Debug.LogFormat("Resizing Screen From {0}x{1} To {2}x{3}", Screen.width, Screen.height, newWidth, newHeight);
                    Screen.SetResolution(newWidth, newHeight, true);
                }
                else if (isLandscape == false && Screen.height > maxScreenSize)
                {
                    float aspectRatio = Screen.width / (float)Screen.height;
                    int newHeight = maxScreenSize;
                    int newWidth = (int)(maxScreenSize * aspectRatio);

                    Debug.LogFormat("Resizing Screen From {0}x{1} To {2}x{3}", Screen.width, Screen.height, newWidth, newHeight);
                    Screen.SetResolution(newWidth, newHeight, true);
                }
            }
        }

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            [SerializeField] private bool limitMobileScreenSize;
            [SerializeField] private int maxScreenSize = 1920;
#pragma warning restore 0649

            public bool LimitMobileScreenSize
            {
                get => this.limitMobileScreenSize;
                set => this.limitMobileScreenSize = value;
            }

            public int MaxScreenSize
            {
                get => this.maxScreenSize;
                set => this.maxScreenSize = value;
            }
        }
    }
}

#endif
