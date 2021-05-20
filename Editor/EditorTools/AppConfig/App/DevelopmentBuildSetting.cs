//-----------------------------------------------------------------------
// <copyright file="DevelopmentBuildSetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.BuildConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(10)]
    public class DevelopmentBuildSetting : AppConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private bool isDevelopmentBuild;
#pragma warning restore 0649

        public bool IsDevelopmentBuild
        {
            get => this.isDevelopmentBuild;
            set => this.isDevelopmentBuild = value;
        }

        public override string DisplayName => "Development Build";

        public override bool IsInline => true;

        public override void InitializeOnLoad(BuildConfig.AppConfig appConfig)
        {
            // var settings = appConfig.GetSettings<DevelopmentBuildSetting>();
            // EditorUserBuildSettings.development = settings.isDevelopmentBuild;
        }

        public override BuildPlayerOptions ChangeBuildPlayerOptions(BuildConfig.AppConfig appConfig, BuildPlayerOptions buildPlayerOptions)
        {
            // var settings = appConfig.GetSettings<DevelopmentBuildSetting>();
            //
            // if (settings.isDevelopmentBuild)
            // {
            //     buildPlayerOptions.options |= BuildOptions.Development;
            // }
            // else
            // {
            //     buildPlayerOptions.options &= ~BuildOptions.Development;
            // }
            //

            return buildPlayerOptions;
        }
    }
}
