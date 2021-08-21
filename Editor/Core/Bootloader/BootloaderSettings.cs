#pragma warning disable

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
        [Header("Bootloader")]
        [SerializeField] private BootloaderLocation bootloaderLocation;
        [SerializeField] private string bootloaderPath = Bootloader.LostBootloaderResourcePath;

        [Header("Managers")]
        [SerializeField] private ManagersLocation managersLocation;
        [SerializeField] private string managersPath = Bootloader.LostManagersResourcePath;

        [Header("Reboot")]
        [SerializeField] private string rebootSceneName = Bootloader.DefaultRebootSceneName;

        [Header("Scenes To Ignore")]
        [Tooltip("A ';' delimited list of scene names that should not startup the bootloader process.")]
        [SerializeField] private string scenesToIgnore;
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

            // Bootloader
            runtimeConfigSettings.Add(Bootloader.BootloaderLocation, ((int)settings.bootloaderLocation).ToString());
            runtimeConfigSettings.Add(Bootloader.BootloaderPath, settings.bootloaderPath);

            // Managers
            runtimeConfigSettings.Add(Bootloader.BootloaderManagersLocation, ((int)settings.managersLocation).ToString());
            runtimeConfigSettings.Add(Bootloader.BootloaderManagersPath, settings.managersPath);

            // Reboot
            runtimeConfigSettings.Add(Bootloader.BootloaderRebootSceneName, settings.rebootSceneName);

            // Ignoring
            runtimeConfigSettings.Add(Bootloader.BootloaderIgnoreSceneNames, settings.scenesToIgnore);
        }

        public override void DrawSettings(BuildConfigSettings settings, SerializedProperty settingsSerializedProperty, float width)
        {
            base.DrawSettings(settings, settingsSerializedProperty, width);
        }
    }
}
