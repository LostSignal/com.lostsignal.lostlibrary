#pragma warning disable

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

    [BuildConfigSettingsOrder(10)]
    public class DevelopmentBuildSetting : BuildConfigSettings
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

        // NOTE [bgish]: Should this be on User/Cloud build initiated instead?
        [EditorEvents.OnDomainReload]
        private static void OnDomainReload()
        {
            //// var settings = EditorAppConfig.GetActiveSettings<DevelopmentBuildSetting>();
            ////
            //// if (settings != null)
            //// {
            ////     EditorUserBuildSettings.development = settings.isDevelopmentBuild;
            //// }
        }

        public override BuildPlayerOptions ChangeBuildPlayerOptions(BuildConfig.BuildConfig buildConfig, BuildPlayerOptions buildPlayerOptions)
        {
            // var settings =  EditorAppConfig.GetActiveSettings<DevelopmentBuildSetting>();
            //
            // if (settings != null)
            // {
            //     if (settings.isDevelopmentBuild)
            //     {
            //         buildPlayerOptions.options |= BuildOptions.Development;
            //     }
            //     else
            //     {
            //         buildPlayerOptions.options &= ~BuildOptions.Development;
            //     }
            // }
            //

            return buildPlayerOptions;
        }
    }
}
