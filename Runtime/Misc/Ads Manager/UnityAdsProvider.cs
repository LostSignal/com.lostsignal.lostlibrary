//-----------------------------------------------------------------------
// <copyright file="UnityAdsProvider.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

#if USING_UNITY_ADS
    using UnityEngine.Advertisements;
#endif

    //// TODO [bgish]: Possible add warnings/errors if they want to use Unity Ads but don't specify a proper Store Id
    //// TODO [bgish]: Investigate the removal of the USING_UNITY_ADS define

#if USING_UNITY_ADS
    public class UnityAdsProvider : MonoBehaviour, IAdProvider, IUnityAdsListener
#else
    public class UnityAdsProvider : MonoBehaviour, IAdProvider
#endif
    {
#pragma warning disable 0649, 0414
        [SerializeField] private string appleAppStoreId = null;
        [SerializeField] private string googlePlayAppStoreId = null;
#pragma warning restore 0649, 0414

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
                }),
            };

            UnityEngine.Advertisements.Advertisement.Show(placementId, options);
        }

        public void OnUnityAdsReady(string placementId)
        {
            throw new System.NotImplementedException();
        }

        public void OnUnityAdsDidError(string message)
        {
            throw new System.NotImplementedException();
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            throw new System.NotImplementedException();
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            throw new System.NotImplementedException();
        }

        private bool IsPlacementIdAdReady(string placementId)
        {
            return UnityEngine.Advertisements.Advertisement.isSupported &&
                UnityEngine.Advertisements.Advertisement.isInitialized &&
                UnityEngine.Advertisements.Advertisement.IsReady(placementId);
        }

#if UNITY_EDITOR && !USING_UNITY_ADS

        [ShowEditorError("This provider will not work unless you the Unity Ads package added through the Package Manager.")]
        [ExposeInEditor("Add Unity Ads Package")]
        private void AddUsingUsingUnityAdsDefine()
        {
            PackageManagerUtil.Add("com.unity.ads");
        }

#elif UNITY_EDITOR && USING_UNITY_ADS

        [ExposeInEditor("Open Unity Ads Dashboard")]
        private void OpenUnityAdsDashboard()
        {
            string projectId = UnityEditor.CloudProjectSettings.projectId;
            string organizationId = UnityEditor.CloudProjectSettings.organizationId;
            string url = $"https://operate.dashboard.unity3d.com/organizations/{organizationId}/projects/{projectId}/operate-settings";
            Application.OpenURL(url);
        }

#endif

        private void OnEnable()
        {
#if USING_UNITY_ADS
            AdsManager.OnInitialized += () =>
            {
                Advertisement.AddListener(this);

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

#endif
