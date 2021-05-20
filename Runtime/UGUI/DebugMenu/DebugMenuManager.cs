//-----------------------------------------------------------------------
// <copyright file="DebugMenuManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using global::PlayFab.ClientModels;
    using global::PlayFab.Internal;
    using Lost.BuildConfig;
    using UnityEngine;

    public class DebugMenuManager : Manager<DebugMenuManager>
    {
#pragma warning disable 0649
        [SerializeField] private bool developmentBuildsOnly = true;

        [Header("Settings")]
        [SerializeField] private DebugMenu.DebugMenuSettings settings = new DebugMenu.DebugMenuSettings();

        [Header("Overlay Options")]
        [SerializeField] private bool showAppVersionInLowerLeftKey = true;
        [SerializeField] private bool showPlayFabIdInLowerRight = true;

        [Header("Debug Menu Options")]
        [SerializeField] private bool showTestAd = true;
        [SerializeField] private bool showToggleFps = true;
        [SerializeField] private bool showPrintAdsInfo = true;
        [SerializeField] private bool addRebootButton = true;
#pragma warning restore 0649

        public override void Initialize()
        {
            if (this.developmentBuildsOnly == false || Application.isEditor || Debug.isDebugBuild)
            {
                this.StartCoroutine(InitializeSettings());
            }
            else
            {
                this.SetInstance(this);
            }

            IEnumerator InitializeSettings()
            {
                yield return DialogManager.WaitForInitialization();

                var debugMenu = DialogManager.GetDialog<DebugMenu>();

                debugMenu.SetSettings(this.settings);

                if (this.showAppVersionInLowerLeftKey)
                {
                    debugMenu.SetText(Corner.LowerLeft, RuntimeBuildConfig.Instance.VersionAndCommitId);
                }

                if (this.showPlayFabIdInLowerRight)
                {
                    PlayFab.PlayFabManager.OnInitialized += () =>
                    {
                        if (PlayFab.PlayFabManager.Instance.Login.IsLoggedIn)
                        {
                            SetPlayFabId();
                        }
                    };
                }

                if (this.showTestAd)
                {
                    debugMenu.AddItem("Show Test Ad", ShowTestAd);
                }

                if (this.showToggleFps)
                {
                    debugMenu.AddItem("Toggle FPS", ToggleFps);
                }

                if (this.showPrintAdsInfo)
                {
                    debugMenu.AddItem("Print Ads Info", PrintAdsInfo);
                }

                if (this.addRebootButton)
                {
                    debugMenu.AddItem("Reboot", Bootloader.Reboot);
                }

                debugMenu.Dialog.Show();

                this.SetInstance(this);
            }
        }

        private static void OnGlobalPlayFabResultHandler(PlayFabRequestCommon request, PlayFabResultCommon result)
        {
            if (result is LoginResult)
            {
                SetPlayFabId();
            }
        }

        private static void SetPlayFabId()
        {
            DialogManager.GetDialog<DebugMenu>().SetText(Corner.LowerRight, PlayFab.PlayFabManager.Instance.Login.IsLoggedIn ? PlayFab.PlayFabManager.Instance.User.PlayFabId : "Login Error!");
        }

        private static void ShowTestAd()
        {
            AdsManager.Instance.ShowAd(null, false, (result) =>
            {
                Debug.Log("ShowAd Result String = " + result.ToString());
            });
        }

        private static void ToggleFps()
        {
            DialogManager.GetDialog<DebugMenu>().ToggleFPS();
        }

        private static void PrintAdsInfo()
        {
#if USING_UNITY_ADS
            Debug.Log("USING_UNITY_ADS Is On");
#else
            Debug.Log("USING_UNITY_ADS Is Off");
#endif

#if UNITY_ADS
            Debug.Log("UNITY_ADS Is On");
#else
            Debug.Log("UNITY_ADS Is Off");
#endif

            Debug.Log("AreAdsInitialized: " + AdsManager.Instance.AreAdsInitialized);
            Debug.Log("AreAdsSupported: " + AdsManager.Instance.AreAdsSupported);
            Debug.Log("CurrentProviderName: " + AdsManager.Instance.CurrentProviderName);

#if UNITY_ADS && (UNITY_IOS || UNITY_ANDROID)
            Debug.Log("UnityEngine.Advertisements.Advertisement.isSupported: " + UnityEngine.Advertisements.Advertisement.isSupported);
            Debug.Log("UnityEngine.Advertisements.Advertisement.isInitialized: " + UnityEngine.Advertisements.Advertisement.isInitialized);
#endif
        }
    }
}
