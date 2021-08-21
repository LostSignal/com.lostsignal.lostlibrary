#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="IAdProvider.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public enum AdWatchedResult
    {
        ProviderNotset,
        AdsNotSupported,
        AdsNotInitialized,
        AdNotReady,
        AdFailed,
        AdSkipped,
        AdFinished,
    }

    public interface IAdProvider
    {
        string ProviderName { get; }
        bool AreAdsSupported { get; }
        bool AreAdsInitialized { get; }
        bool IsAdReady(string placementId);
        void ShowAd(string placementId, bool isRewarded, System.Action<AdWatchedResult> watchResultCallback);
    }
}

#endif
