//-----------------------------------------------------------------------
// <copyright file="BuildPlayerContentSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Addressables
{
    using Lost.BuildConfig;
    using UnityEngine;

    [BuildConfigSettingsOrder(7)]
    public class BuildPlayerContentSettings : BuildConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private bool buildPlayerContentOnBuild = true;
#pragma warning restore 0649

        public override string DisplayName => "Build Player Content";

        public override bool IsInline => true;

        [EditorEvents.OnUserBuildInitiated]
        [EditorEvents.OnCloudBuildInitiated]
        private static void BuildPlayerContent()
        {
            var settings = EditorBuildConfigs.GetActiveSettings<BuildPlayerContentSettings>();

            if (settings != null && settings.buildPlayerContentOnBuild)
            {
                UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
            }
        }
    }
}
