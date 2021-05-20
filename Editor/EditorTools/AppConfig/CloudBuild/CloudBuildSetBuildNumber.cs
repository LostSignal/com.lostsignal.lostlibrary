//-----------------------------------------------------------------------
// <copyright file="CloudBuildSetBuildNumber.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using Lost.CloudBuild;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    [AppConfigSettingsOrder(275)]
    public class CloudBuildSetBuildNumber : AppConfigSettings
    {
        public enum BuildNumberType
        {
            ScmCommitNumber,
            CloudBuildNumber,
        }

#pragma warning disable 0649
        [Tooltip("SCM Commit Numer only works for Perfoce and PlasticSCM, and Cloud Build Number works for all source control types.")]
        [SerializeField] private BuildNumberType buildNumberType;
        [SerializeField] private int incrementBuildNumberBy;
#pragma warning restore 0649

        public override string DisplayName => "CloudBuild - Set Build Number";
        public override bool IsInline => false;

        public override void OnPreproccessBuild(AppConfig.AppConfig appConfig, BuildReport buildReport)
        {
            var settings = appConfig.GetSettings<CloudBuildSetBuildNumber>();

            if (settings == null)
            {
                return;
            }

            int buildNumber = this.GetBuildNumber(settings);

            if (buildNumber != -1)
            {
                PlayerSettings.iOS.buildNumber = buildNumber.ToString();
                PlayerSettings.Android.bundleVersionCode = buildNumber;
            }
        }

        private int GetBuildNumber(CloudBuildSetBuildNumber settings)
        {
            if (Platform.IsUnityCloudBuild == false)
            {
                // NOTE [bgish]: Gradle Build will fail if build number is 0, so returning 1
                // android.defaultConfig.versionCode is set to 0, but it should be a positive integer.
                return 1;
            }

            var cloudBuildManifest = CloudBuildManifest.Find();

            if (cloudBuildManifest == null)
            {
                Debug.LogError("CloudBuildSetBuildNumber couldn't find CloudBuildManifest!");
            }
            else if (settings.buildNumberType == BuildNumberType.CloudBuildNumber)
            {
                Debug.LogFormat("CloudBuildSetBuildNumber setting application build number to unity cloud CloudBuildNumber {0}!", cloudBuildManifest.BuildNumber);
                return cloudBuildManifest.BuildNumber + settings.incrementBuildNumberBy;
            }
            else if (settings.buildNumberType == BuildNumberType.ScmCommitNumber)
            {
                string commitId = cloudBuildManifest.ScmCommitId;

                if (int.TryParse(commitId, out int commitNumber))
                {
                    Debug.LogFormat("CloudBuildSetBuildNumber setting application build number to ScmCommitId {0}!", commitId);
                    return commitNumber + settings.incrementBuildNumberBy;
                }
                else
                {
                    Debug.LogErrorFormat("CloudBuildSetBuildNumber couldn't parse ScmCommitId {0}.  It is not a valid integer!", commitId);
                }
            }
            else
            {
                Debug.LogErrorFormat("Found unknown BuildNumberType {0}", settings.buildNumberType);
            }

            return -1;
        }
    }
}
