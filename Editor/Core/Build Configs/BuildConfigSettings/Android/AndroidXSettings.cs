﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="AndroidXSetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using Lost.BuildConfig;
    using UnityEngine;

    [BuildConfigSettingsOrder(305)]
    public class AndroidXSetting : BuildConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool overrideGradleProperties = true;
        [SerializeField] private bool useAndroidX = true;
        [SerializeField] private bool enableJetifier = true;
        #pragma warning restore 0649

        public override string DisplayName => "Adroid X";
        public override bool IsInline => false;

        ////
        //// https://stackoverflow.com/questions/54186051/is-there-a-way-to-change-the-gradle-properties-file-in-unity
        ////
        [EditorEvents.OnPostGenerateGradleAndroidProject]
        private static void OnPostGenerateGradleAndroidProject(string gradlePath)
        {
            var settings = EditorBuildConfigs.GetActiveSettings<AndroidXSetting>();

            if (settings == null || settings.overrideGradleProperties == false)
            {
                return;
            }

            string gradlePropertiesFile = gradlePath + "/gradle.properties";

            if (File.Exists(gradlePropertiesFile))
            {
                File.Delete(gradlePropertiesFile);
            }

            StreamWriter writer = File.CreateText(gradlePropertiesFile);
            writer.WriteLine("org.gradle.jvmargs=-Xmx4096M");
            writer.WriteLine("android.useAndroidX=" + settings.useAndroidX.ToString().ToLower());
            writer.WriteLine("android.enableJetifier=" + settings.enableJetifier.ToString().ToLower());
            writer.Flush();
            writer.Close();
        }
    }
}
