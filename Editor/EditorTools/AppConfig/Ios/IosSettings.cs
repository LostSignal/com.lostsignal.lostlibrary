//-----------------------------------------------------------------------
// <copyright file="IosSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.BuildConfig;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    [AppConfigSettingsOrder(420)]
    public class IosSettings : AppConfigSettings
    {
        public enum IOSPushNotificationType
        {
            None,
            Development,
            Production,
        }

        // Build step to add<key>ITSAppUsesNonExemptEncryption</key><false/> to info.plist???

        #pragma warning disable 0649
        [SerializeField] private bool disableIOSBitCode;
        [SerializeField] private IOSPushNotificationType iosPushNotificationType;
        #pragma warning restore 0649

        public override string DisplayName => "iOS Settings";
        public override bool IsInline => false;

        public override void OnPostprocessBuild(BuildConfig.AppConfig buildConfig, BuildReport buildReport)
        {
            var settings = buildConfig.GetSettings<IosSettings>();
            var path = buildReport.summary.outputPath;

            if (settings == null || buildReport.summary.platform != UnityEditor.BuildTarget.iOS)
            {
                return;
            }

            this.DisableBitCode(settings, path);
            this.EnableIOSPushNotifications(settings, path);
        }

        private void DisableBitCode(IosSettings settings, string path)
        {
            if (settings.disableIOSBitCode == false)
            {
                return;
            }

            Debug.Log("Disabling BitCode...");

            #if UNITY_IOS
            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            var pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
            pbxProject.ReadFromFile(projectPath);

            string target = pbxProject.TargetGuidByName("Unity-iPhone");
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            pbxProject.WriteToFile(projectPath);
            #endif
        }

        // Majority of this code was thanks to the com.unity.mobile.notifications package
        private void EnableIOSPushNotifications(IosSettings settings, string buildPath)
        {
            #if UNITY_IOS

            if (settings.iosPushNotificationType == IOSPushNotificationType.None)
            {
                return;
            }

            // Turning on push notifications (release/development)
            var projectPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var project = new UnityEditor.iOS.Xcode.PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath));

            // Push Notification Capability
            var manager = new UnityEditor.iOS.Xcode.ProjectCapabilityManager(
                projectPath,
                "Entitlements.entitlements",
                targetGuid: project.GetUnityMainTargetGuid()
            );
            manager.AddPushNotifications(settings.iosPushNotificationType == IOSPushNotificationType.Development);
            manager.WriteToFile();

            // Making sure Uses Remote Notifications is on
            var preprocessorPath = buildPath + "/Classes/Preprocessor.h";
            var preprocessor = System.IO.File.ReadAllText(preprocessorPath);
            preprocessor = preprocessor.Replace("UNITY_USES_REMOTE_NOTIFICATIONS 0", "UNITY_USES_REMOTE_NOTIFICATIONS 1");

            System.IO.File.WriteAllText(preprocessorPath, preprocessor);

            #endif
        }
    }
}
