//-----------------------------------------------------------------------
// <copyright file="BuildPlayerContentSettings.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Addressables
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(7)]
    public class BuildPlayerContentSettings : AppConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private bool buildPlayerContentOnBuild = true;
#pragma warning restore 0649

        public override string DisplayName => "Build Player Content";

        public override bool IsInline => true;

        public override void OnUnityCloudBuildInitiated(AppConfig appConfig)
        {
            this.BuildPlayerContent(appConfig);
        }

        public override void OnUserBuildInitiated(AppConfig appConfig)
        {
            this.BuildPlayerContent(appConfig);
        }

        private void BuildPlayerContent(AppConfig appConfig)
        {
            var settings = EditorAppConfig.ActiveAppConfig?.GetSettings<BuildPlayerContentSettings>();

            if (settings.buildPlayerContentOnBuild)
            {
                UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
            }
        }
    }
}
