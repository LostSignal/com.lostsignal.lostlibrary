//-----------------------------------------------------------------------
// <copyright file="Release.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using Lost.DissonanceIntegration;
    using Lost.PlayFab;
    using Lost.IAP;
    using UnityEngine;

    [Serializable]
    public class Release
    {
#pragma warning disable 0649
        [SerializeField] private string appVersion = "0.1.0";
        [SerializeField] private string dataVersion = "0";
        [SerializeField] private bool forceAppUpdate;
        [SerializeField] private bool isXRApp;

        [Header("Manager Settings")]
        [SerializeField] private DissonanceManager.Settings dissonanceManagerSettings = new DissonanceManager.Settings();
        [SerializeField] private LoggingManager.Settings loggingManagerSettings = new LoggingManager.Settings();
        [SerializeField] private PlayFabManager.Settings playfabManagerSettings = new PlayFabManager.Settings();
        [SerializeField] private RealtimeMessageManager.Settings realtimeMessageManagerSettings = new RealtimeMessageManager.Settings();
        [SerializeField] private ScreenSizeManager.Settings screenSizeManagerSettings = new ScreenSizeManager.Settings();
        [SerializeField] private SpriteAtlasLoadingManager.Settings spriteAtlasLoadingManagerSettings = new SpriteAtlasLoadingManager.Settings();
        [SerializeField] private UnityPurchasingManager.Settings unityPurchasingManagerSettings = new UnityPurchasingManager.Settings();
#pragma warning restore 0649

        public string AppVersion
        {
            get => this.appVersion;
            set => this.appVersion = value;
        }

        public string DataVersion
        {
            get => this.dataVersion;
            set => this.dataVersion = value;
        }

        public bool ForceAppUpdate
        {
            get => this.forceAppUpdate;
            set => this.forceAppUpdate = value;
        }

        public bool IsXRApp
        {
            get => this.isXRApp;
            set => this.isXRApp = value;
        }

        public PlayFabManager.Settings PlayfabManagerSettings
        {
            get => this.playfabManagerSettings;
            set => this.playfabManagerSettings = value;
        }

        public RealtimeMessageManager.Settings RealtimeMessageManagerSettings
        {
            get => this.realtimeMessageManagerSettings;
            set => this.realtimeMessageManagerSettings = value;
        }

        public SpriteAtlasLoadingManager.Settings SpriteAtlasLoadingManagerSettings
        {
            get => this.spriteAtlasLoadingManagerSettings;
            set => this.spriteAtlasLoadingManagerSettings = value;
        }

        public ScreenSizeManager.Settings ScreenSizeManagerSettings
        {
            get => this.screenSizeManagerSettings;
            set => this.screenSizeManagerSettings = value;
        }

        public LoggingManager.Settings LoggingManagerSettings
        {
            get => this.loggingManagerSettings;
            set => this.loggingManagerSettings = value;
        }

        public UnityPurchasingManager.Settings UnityPurchasingManagerSettings
        {
            get => this.unityPurchasingManagerSettings;
            set => this.unityPurchasingManagerSettings = value;
        }

        public DissonanceManager.Settings DissonanceManagerSettings
        {
            get => this.dissonanceManagerSettings;
            set => this.dissonanceManagerSettings = value;
        }
    }
}

#endif
