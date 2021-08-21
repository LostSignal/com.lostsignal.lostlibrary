#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ContextMenuTools.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public static class ContextMenuTools
    {
        [MenuItem("Assets/Lost/Add AnimationClip as Sub-Object", true, 100)]
        public static bool AddSubAnimationClipToAnimatorValidator()
        {
            return Selection.objects.Length == 1 && Selection.objects.FirstOrDefault() is AnimatorController;
        }

        [MenuItem("Assets/Lost/Add AnimationClip as Sub-Object", false, 100)]
        public static void AddSubAnimationClipToAnimator()
        {
            var animationController = Selection.objects.FirstOrDefault() as AnimatorController;

            if (Provider.isActive)
            {
                Provider.Checkout(animationController, CheckoutMode.Asset);
            }

            AssetDatabase.AddObjectToAsset(new AnimationClip() { name = "New AnimationClip" }, animationController);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationController));
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Lost/Add Show And Hide Sub Animations", true, 101)]
        public static bool AddChildAnimationClipValidator()
        {
            return Selection.activeObject is AnimatorController;
        }

        [MenuItem("Assets/Lost/Add Show And Hide Sub Animations", false, 101)]
        public static void AddChildAnimationClip()
        {
            AssetDatabase.AddObjectToAsset(new AnimationClip() { name = "Show" }, Selection.activeObject);
            AssetDatabase.AddObjectToAsset(new AnimationClip() { name = "Hide" }, Selection.activeObject);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem("Assets/Lost/Add TextObject as Sub-Object", true, 101)]
        public static bool AddTextObjectAsSubObjectValidaate()
        {
            return Selection.objects.Length == 1 && Selection.objects.FirstOrDefault() is ScriptableObject;
        }

        [MenuItem("Assets/Lost/Add TextObject as Sub-Object", false, 101)]
        public static void AddTextObjectAsSubObjectExecute()
        {
            var scriptableObject = Selection.objects.FirstOrDefault() as ScriptableObject;

            if (Provider.isActive)
            {
                Provider.Checkout(scriptableObject, CheckoutMode.Asset);
            }

            AssetDatabase.AddObjectToAsset(new TextObject() { name = "New TextObject" }, scriptableObject);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(scriptableObject));
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Lost/Delete Sub-Object", true, 102)]
        public static bool DeleteSubObjectValidate()
        {
            var obj = Selection.objects.FirstOrDefault();
            var objParent = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GetAssetPath(obj));

            return obj != objParent;
        }

        [MenuItem("Assets/Lost/Delete Sub-Object", false, 102)]
        public static void DeleteSubObjectExecute()
        {
            var obj = Selection.objects.FirstOrDefault();
            var objParent = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GetAssetPath(obj));

            if (Provider.isActive)
            {
                Provider.Checkout(objParent, CheckoutMode.Asset);
            }

            AssetDatabase.RemoveObjectFromAsset(obj);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(objParent));
        }

        [MenuItem("Assets/Lost/Generate Thumbnail Image", false, 120)]
        public static void GenerateThumbnailImage()
        {
            foreach (var obj in Selection.objects)
            {
                var objFilePath = AssetDatabase.GetAssetPath(obj);
                var imageFilePath = Path.Combine(Path.GetDirectoryName(objFilePath), Path.GetFileNameWithoutExtension(objFilePath) + ".png");
                var thumbnailTexture = AssetPreview.GetAssetPreview(obj);

                if (thumbnailTexture != null)
                {
                    if (File.Exists(imageFilePath))
                    {
                        var asset = Provider.GetAssetByPath(imageFilePath);

                        if (Provider.CheckoutIsValid(asset))
                        {
                            Provider.Checkout(asset, CheckoutMode.Asset).Wait();
                        }
                    }

                    File.WriteAllBytes(imageFilePath, ImageConversion.EncodeToPNG(thumbnailTexture));
                }
                else
                {
                    Debug.LogError($"Unable to create thumbnail {imageFilePath}");
                }
            }
        }

        [MenuItem("Assets/Lost/Copy GUID", false, 121)]
        public static void PrintGuid()
        {
            if (Selection.assetGUIDs.Length == 1)
            {
                EditorGUIUtility.systemCopyBuffer = Selection.assetGUIDs[0];
            }
        }

        [MenuItem("Assets/Lost/Regenerate All Folder Guids", false, 122)]
        public static void DeleteAllFolderMetaFiles()
        {
            if (Selection.activeObject == null)
            {
                UnityEngine.Debug.LogError("You must select a directory for this option to work.");
                return;
            }

            string rootDirectoryAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            if (Directory.Exists(rootDirectoryAssetPath) == false)
            {
                UnityEngine.Debug.LogError("You must select a directory for this option to work.");
                return;
            }

            foreach (var directory in Directory.GetDirectories(rootDirectoryAssetPath, "*", SearchOption.AllDirectories))
            {
                var assetPath = directory.Replace("\\", "/");
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                var newGuid = Guid.NewGuid().ToString("N");

                string metaFilePath = assetPath + ".meta";

                if (File.Exists(metaFilePath))
                {
                    // NOTE [bgish]: Not sure why, but this only seems to get half the folders
                    // Provider.Checkout(asset, CheckoutMode.Both).Wait();

                    File.WriteAllText(metaFilePath, File.ReadAllText(metaFilePath).Replace(assetGuid, newGuid));
                }
            }
        }

        [MenuItem("Assets/Lost/Get Lines of Code Count", false, 123)]
        public static void LinesOfCode()
        {
            if (Selection.activeObject == null)
            {
                UnityEngine.Debug.LogError("You must select a directory for this option to work.");
                return;
            }

            string rootDirectoryAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            if (Directory.Exists(rootDirectoryAssetPath) == false)
            {
                UnityEngine.Debug.LogError("You must select a directory for this option to work.");
                return;
            }

            int linesOfCodeCount = 0;
            int fileCount = 0;

            foreach (var file in Directory.GetFiles(rootDirectoryAssetPath, "*.cs", SearchOption.AllDirectories))
            {
                fileCount++;

                foreach (var line in File.ReadAllLines(file))
                {
                    string trimmedLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine == "{" || trimmedLine == "}" || trimmedLine.StartsWith("//"))
                    {
                        continue;
                    }

                    linesOfCodeCount++;
                }
            }

            UnityEngine.Debug.Log("C# File Count: " + fileCount);
            UnityEngine.Debug.Log("Line Count: " + linesOfCodeCount);
        }
    }
}
