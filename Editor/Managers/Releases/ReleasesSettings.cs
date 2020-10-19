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
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(15)]
    public class ReleasesSettings : AppConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private Releases releases;
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

        public static Releases Releases
        {
            get
            {
                string releasesNamespace = "com.lostsignal.releases";

                if (EditorBuildSettings.TryGetConfigObject(releasesNamespace, out Releases releases) == false || !releases)
                {
                    string releasesDirectory = "Assets/Editor/com.lostsignal.lostlibrary";
                    string releasesAssetName = "Releases.asset";
                    string releasesAssetPath = Path.Combine(releasesDirectory, releasesAssetName);

                    if (Directory.Exists(releasesDirectory) == false)
                    {
                        Directory.CreateDirectory(releasesDirectory);
                    }

                    Releases releasesObject;

                    if (File.Exists(releasesAssetPath) == false)
                    {
                        releasesObject = ScriptableObject.CreateInstance<Releases>();
                        AssetDatabase.CreateAsset(releasesObject, releasesAssetPath);
                        EditorUtility.SetDirty(releasesObject);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        releasesObject = AssetDatabase.LoadAssetAtPath<Releases>(releasesAssetPath);
                    }

                    EditorBuildSettings.AddConfigObject(releasesNamespace, releasesObject, true);

                    return releasesObject;
                }
                else
                {
                    return releases;
                }
            }
        }

        public override string DisplayName => "Releases Settings";

        public override bool IsInline => false;

        public override void InitializeOnLoad(AppConfig.AppConfig buildConfig)
        {
            var settings = buildConfig.GetSettings<ReleasesSettings>();

            if (settings == null)
            {
                return;
            }
        }

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            base.GetRuntimeConfigSettings(appConfig, runtimeConfigSettings);

            var settings = appConfig.GetSettings<ReleasesSettings>();

            if (settings == null)
            {
                return;
            }

            runtimeConfigSettings.Add(ReleasesManager.ReleasesMachineNameKey, System.Environment.MachineName);
            runtimeConfigSettings.Add(ReleasesManager.ReleasesUrlKey, settings.releasesUrl);
            runtimeConfigSettings.Add(ReleasesManager.ReleasesCurrentRelease, JsonUtil.Serialize(Releases.CurrentRelease));
        }

        public override void OnUserBuildInitiated(AppConfig.AppConfig appConfig)
        {
            base.OnUserBuildInitiated(appConfig);
            this.UploadReleases(appConfig);
        }

        public override void OnUnityCloudBuildInitiated(AppConfig.AppConfig appConfig)
        {
            base.OnUnityCloudBuildInitiated(appConfig);
            this.UploadReleases(appConfig);
        }

        private void UploadReleases(AppConfig.AppConfig appConfig)
        {
            var settings = appConfig.GetSettings<ReleasesSettings>();

            if (settings == null || settings.disableUplaod)
            {
                return;
            }

            var machineName = Platform.IsUnityCloudBuild ? "cloud_build" : System.Environment.MachineName;
            var blobKey = $"{machineName}/{ReleasesManager.ReleasesJsonFileName}";
            var releasesJson = JsonUtil.Serialize(this.releases);

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
