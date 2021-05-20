//-----------------------------------------------------------------------
// <copyright file="BundleIdentifierSetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.BuildConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(5)]
    public class BundleIdentifierSetting : AppConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private string bundleId;
#pragma warning restore 0649

        public override string DisplayName => "Bundle Identifier";

        public override bool IsInline => true;

        public override void InitializeOnLoad(BuildConfig.AppConfig appConfig)
        {
            var setting = appConfig.GetSettings<BundleIdentifierSetting>();

            foreach (var buildTargetGroup in BuildTargetGroupUtil.GetValid())
            {
                var currentBundleId = PlayerSettings.GetApplicationIdentifier(buildTargetGroup);

                if (string.IsNullOrEmpty(setting.bundleId))
                {
                    if (string.IsNullOrEmpty(currentBundleId) == false)
                    {
                        setting.bundleId = currentBundleId;
                        break;
                    }
                }
                else if (setting.bundleId != currentBundleId)
                {
                    PlayerSettings.SetApplicationIdentifier(buildTargetGroup, setting.bundleId);
                }
            }
        }
    }
}
