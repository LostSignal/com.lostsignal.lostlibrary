//-----------------------------------------------------------------------
// <copyright file="LostLibrarySettingsAssetPostprocessor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;

    public class LostLibrarySettingsAssetPostprocessor : AssetPostprocessor
    {
        private static void OnGeneratedCSProjectFiles()
        {
            LostLibrarySettings.Instance.AddEditorConfigToSolution();
            LostLibrarySettings.Instance.AddAnalyzersToCSProjects();
        }
    }
}
