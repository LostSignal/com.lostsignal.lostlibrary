//-----------------------------------------------------------------------
// <copyright file="LostLibrarySettingsAssetProcessor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;

    public class LostLibrarySettingsAssetProcessor : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string assetPath)
        {
            LostLibrarySettings.Instance.OverrideCSharpTemplateFiles(assetPath);
        }

        private static void OnGeneratedCSProjectFiles()
        {
            LostLibrarySettings.Instance.AddEditorConfigToSolution();
            LostLibrarySettings.Instance.AddAnalyzersToCSProjects();
        }
    }
}
