//-----------------------------------------------------------------------
// <copyright file="UnityAdsProvider.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    //// TODO [bgish]: Possible add warnings/errors if they want to use Unity Ads but don't specify a proper Store Id
    //// TODO [bgish]: Investigate the removal of the USING_UNITY_ADS define

    public class UnityAdsProvider : MonoBehaviour, IAdProvider
    {
        #pragma warning disable 0649, 0414
        [SerializeField] private string appleAppStoreId = null;
        [SerializeField] private string googlePlayAppStoreId = null;
        #pragma warning restore 0649, 0414

        #if UNITY_EDITOR
        [ExposeInEditor("Open Unity Ads Dashboard")]
        private void OpenUnityAdsDashboard()
        {
            string projectId = UnityEditor.CloudProjectSettings.projectId;
            string organizationId = UnityEditor.CloudProjectSettings.organizationId;
            string url = $"https://operate.dashboard.unity3d.com/organizations/{organizationId}/projects/{projectId}/operate-settings";
            Application.OpenURL(url);
        }

        #endif

        #if !USING_UNITY_ADS
        [ExposeInEditor("Add USING_UNITY_ADS Define")]
        private void AddUsingUsingUnityAdsDefine()
        {
            ProjectDefinesHelper.AddDefineToProject("USING_UNITY_ADS");
        }

        #endif

        private void OnEnable()
        {
            #if !USING_UNITY_ADS

            Debug.LogError("Trying to UnityAdsProvider without the Unity Ads Package");

            #else

            AdsManager.OnInitialized += () =>
            {
                #if UNITY_IOS

                if (string.IsNullOrWhiteSpace(this.appleAppStoreId) == false && UnityEngine.Advertisements.Advertisement.isInitialized == false)
                {
                    UnityEngine.Advertisements.Advertisement.Initialize(this.appleAppStoreId);
                }

                #elif UNITY_ANDROID

                if (string.IsNullOrWhiteSpace(this.googlePlayAppStoreId) == false && UnityEngine.Advertisements.Advertisement.isInitialized == false)
                {
                    UnityEngine.Advertisements.Advertisement.Initialize(this.googlePlayAppStoreId);
                }

                #endif

                AdsManager.Instance.SetAdProvider(this);
            };

            #endif
        }

        string IAdProvider.ProviderName
        {
            get { return "UnityAds"; }
        }

        #if USING_UNITY_ADS && (UNITY_IOS || UNITY_ANDROID)

        bool IAdProvider.AreAdsSupported
        {
            get { return UnityEngine.Advertisements.Advertisement.isSupported; }
        }

        bool IAdProvider.AreAdsInitialized
        {
            get { return UnityEngine.Advertisements.Advertisement.isSupported && UnityEngine.Advertisements.Advertisement.isInitialized; }
        }

        bool IAdProvider.IsAdReady(string placementId)
        {
            return this.IsPlacementIdAdReady(placementId);
        }

        void IAdProvider.ShowAd(string placementId, bool isRewarded, System.Action<AdWatchedResult> watchResultCallback)
        {
            var options = new UnityEngine.Advertisements.ShowOptions()
            {
                resultCallback = new System.Action<UnityEngine.Advertisements.ShowResult>(result =>
                {
                    if (watchResultCallback != null)
                    {
                        switch (result)
                        {
                            case UnityEngine.Advertisements.ShowResult.Failed:
                                watchResultCallback(AdWatchedResult.AdFailed);
                                break;
                            case UnityEngine.Advertisements.ShowResult.Skipped:
                                watchResultCallback(AdWatchedResult.AdSkipped);
                                break;
                            case UnityEngine.Advertisements.ShowResult.Finished:
                                watchResultCallback(AdWatchedResult.AdFinished);
                                break;
                            default:
                                UnityEngine.Debug.LogErrorFormat("UnityAdProvider.ShowAd() encountered unknown ShowResult {0}", result);
                                break;
                        }
                    }
                })
            };

            UnityEngine.Advertisements.Advertisement.Show(placementId, options);
        }

        private bool IsPlacementIdAdReady(string placementId)
        {
            return UnityEngine.Advertisements.Advertisement.isSupported &&
                UnityEngine.Advertisements.Advertisement.isInitialized &&
                UnityEngine.Advertisements.Advertisement.IsReady(placementId);
        }

        #else

        bool IAdProvider.AreAdsSupported => false;

        bool IAdProvider.AreAdsInitialized => false;

        bool IAdProvider.IsAdReady(string placementId)
        {
            return false;
        }

        void IAdProvider.ShowAd(string placementId, bool isRewarded, System.Action<AdWatchedResult> watchResultCallback)
        {
            watchResultCallback?.Invoke(AdWatchedResult.AdsNotSupported);
        }

        #endif
    }
}
