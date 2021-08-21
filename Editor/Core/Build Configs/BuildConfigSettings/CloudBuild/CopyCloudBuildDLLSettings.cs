#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="CopyCloudBuildDLLSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using Lost.BuildConfig;
    using UnityEditor;
    using UnityEngine;

    [BuildConfigSettingsOrder(15)]
    public class CopyCloudBuildDLLSettings : BuildConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool copyCloudBuildDLLToStreamingAssets = true;
        #pragma warning restore 0649

        public override string DisplayName => "Copy CloudBuild DLL To StreamingAssets";
        public override bool IsInline => true;

        [EditorEvents.OnPreprocessBuild]
        private static void OnPreproccessBuild()
        {
            var settings = EditorBuildConfigs.GetActiveSettings<CopyCloudBuildDLLSettings>();

            if (settings == null || settings.copyCloudBuildDLLToStreamingAssets == false || Platform.IsUnityCloudBuild == false)
            {
                return;
            }

            foreach (var file in Directory.GetFiles(".", "*", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file) == "UnityEditor.CloudBuild.dll")
                {
                    // Making sure the directory path exists and copying it over
                    string copyPath = "Assets/StreamingAssets/UnityEditor.CloudBuild.dll.copy";
                    Directory.CreateDirectory(Path.GetDirectoryName(copyPath));
                    File.Copy(file, copyPath);

                    // Importing the asset so it will be included in the build
                    AssetDatabase.ImportAsset(copyPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
