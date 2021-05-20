//-----------------------------------------------------------------------
// <copyright file="ReleasesSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using Lost.Addressables;
    using Lost.BuildConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(15)]
    public class ReleasesSettings : AppConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private string releasesUrl;

        [Header("Azure Upload Settings")]
        [SerializeField] private bool disableUplaod;
        [SerializeField] private string containerName;
        [SerializeField] private string storageAccountName;
        [SerializeField] private string storageAccountKey;
#pragma warning restore 0649

        public ReleasesSettings()
        {
            this.disableUplaod = true;
        }

        public override string DisplayName => "Releases Settings";

        public override bool IsInline => false;

        public override void InitializeOnLoad(BuildConfig.AppConfig buildConfig)
        {
            var settings = buildConfig.GetSettings<ReleasesSettings>();

            if (settings == null)
            {
                return;
            }
        }

        public override void GetRuntimeConfigSettings(BuildConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            base.GetRuntimeConfigSettings(appConfig, runtimeConfigSettings);

            var settings = appConfig.GetSettings<ReleasesSettings>();

            if (settings == null)
            {
                return;
            }

            runtimeConfigSettings.Add(ReleasesManager.ReleasesMachineNameKey, System.Environment.MachineName);
            runtimeConfigSettings.Add(ReleasesManager.ReleasesUrlKey, settings.releasesUrl);
            runtimeConfigSettings.Add(ReleasesManager.ReleasesCurrentRelease, JsonUtil.Serialize(LostLibrary.Releases.CurrentRelease));
        }

        public override void OnUserBuildInitiated(BuildConfig.AppConfig appConfig)
        {
            base.OnUserBuildInitiated(appConfig);
            this.UploadReleases(appConfig);
        }

        public override void OnUnityCloudBuildInitiated(BuildConfig.AppConfig appConfig)
        {
            base.OnUnityCloudBuildInitiated(appConfig);
            this.UploadReleases(appConfig);
        }

        private void UploadReleases(BuildConfig.AppConfig appConfig)
        {
            var settings = appConfig.GetSettings<ReleasesSettings>();

            if (settings == null || settings.disableUplaod)
            {
                return;
            }

            var machineName = Platform.IsUnityCloudBuild ? "cloud_build" : System.Environment.MachineName;
            var blobKey = $"{machineName}/{ReleasesManager.ReleasesJsonFileName}";
            var releasesJson = JsonUtil.Serialize(LostLibrary.Releases.AllReleases);

            var azureConfig = new AzureStorage.Config
            {
                ContainerName = this.containerName,
                StorageAccountName = this.storageAccountName,
                StorageAccountKey = this.storageAccountKey,
            };

            AzureStorage.UploadFile(azureConfig, blobKey, releasesJson.GetUTF8Bytes());
        }
    }
}
