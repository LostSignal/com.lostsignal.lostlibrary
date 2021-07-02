//-----------------------------------------------------------------------
// <copyright file="BootloaderBuildConfig.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.BuildConfig;
    using UnityEditor;
    using UnityEngine;

    [BuildConfigSettingsOrder(8)]
    public class BootloaderSettings : BuildConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private string bootloaderResourcePath = Bootloader.LostBootloaderResourcePath;
        [SerializeField] private string rebootSceneName = Bootloader.DefaultRebootSceneName;
        [SerializeField] private Bootloader.ManagersLocation managersLocation;
        [SerializeField] private string managersPath = Bootloader.LostManagersResourcePath;
#pragma warning restore 0649

        public override string DisplayName => "Bootloader Settings";

        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(BuildConfig.BuildConfig buildConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = buildConfig.GetSettings<BootloaderSettings>();

            if (settings == null)
            {
                return;
            }
            
            runtimeConfigSettings.Add(Bootloader.BootloaderResourcePathSetting, settings.bootloaderResourcePath);
            runtimeConfigSettings.Add(Bootloader.BootloaderManagersLocation, ((int)settings.managersLocation).ToString());
            runtimeConfigSettings.Add(Bootloader.BootloaderManagersPath, settings.managersPath);
            runtimeConfigSettings.Add(Bootloader.BootloaderRebootSceneName, settings.rebootSceneName);
        }

        public override void DrawSettings(BuildConfigSettings settings, SerializedProperty settingsSerializedProperty, float width)
        {
            base.DrawSettings(settings, settingsSerializedProperty, width);
        }
    }
}
