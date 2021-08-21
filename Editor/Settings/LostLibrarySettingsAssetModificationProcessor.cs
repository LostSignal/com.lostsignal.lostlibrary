#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LostLibrarySettingsAssetModificationProcessor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;

    public class LostLibrarySettingsAssetModificationProcessor : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string assetPath)
        {
            LostLibrarySettings.Instance.OverrideCSharpTemplateFiles(assetPath);
        }
    }
}
