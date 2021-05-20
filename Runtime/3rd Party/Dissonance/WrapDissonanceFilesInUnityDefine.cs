//-----------------------------------------------------------------------
// <copyright file="WrapDissonanceFilesInUnityDefine.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE && UNITY_EDITOR

namespace Lost.DissonanceIntegration
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public static class WrapDissonanceFilesInUnityDefine
    {
        private static HashSet<string> nonUnitySpecificFiles = new HashSet<string>
        {
            "Log.cs",
            "LogCategory.cs",
            "LogLevel.cs",
            "DissonanceException.cs",
            "ChannelType.cs",
            "CodecSettings.cs",
            "Codec.cs",
            "Rooms.cs",
            "DissonanceRootPath.cs",
            "IChannelPriorityManager.cs",
            "PlaybackOptions.cs",
            "RoomIdConversion.cs",
        };

        private static HashSet<string> unitySpecificFiles = new HashSet<string>()
        {
            "VoiceSettings.cs",
            "BaseCommsNetwork.cs",
            "Preferences.cs",
        };

        private static HashSet<string> nonUnitySpecificFolders = new HashSet<string>
        {
            "Networking",
            "Extensions",
            "Datastructures",
            "Threading",
            "Config",
            "Channel",
        };

        [MenuItem("Tools/Lost/Dissonance/Wrap Dissonance Files In Unity Define")]
        public static void WrapFilesInDefine()
        {
            var dissonanceRuntimeFolderGuid = "3e9e762569660d9468739176902cddc7";
            var runtimeFolderPath = AssetDatabase.GUIDToAssetPath(dissonanceRuntimeFolderGuid);

            var unityDefine = "#if UNITY_2018_4_OR_NEWER";
            var endUnityDefine = "#endif";

            foreach (var file in Directory.GetFiles(runtimeFolderPath, "*.cs", SearchOption.AllDirectories))
            {
                var fullPath = file.Replace("\\", "/");
                var fileName = Path.GetFileName(file);

                // If we know this is a unity file, then add the ifdef
                if (unitySpecificFiles.Contains(fileName))
                {
                    AddIfDef(file);
                    continue;
                }

                // Skipping NonUnity Files
                if (nonUnitySpecificFiles.Contains(fileName))
                {
                    continue;
                }

                // Skipping NonUnity Folders
                bool isNonUnityFolder = false;
                foreach (var nonUnityFolder in nonUnitySpecificFolders)
                {
                    if (fullPath.Contains($"/{nonUnityFolder}/"))
                    {
                        isNonUnityFolder = true;
                        break;
                    }
                }

                if (isNonUnityFolder)
                {
                    continue;
                }

                AddIfDef(file);
            }

            void AddIfDef(string abosoluteFilePath)
            {
                var fileContents = File.ReadAllText(abosoluteFilePath);

                if (fileContents.StartsWith(unityDefine))
                {
                    return;
                }

                Debug.Log($"Editing file {abosoluteFilePath}");

                StringBuilder newFileContents = new StringBuilder();
                newFileContents.AppendLine(unityDefine);
                newFileContents.AppendLine();
                newFileContents.AppendLine(fileContents);
                newFileContents.AppendLine(endUnityDefine);

                File.WriteAllText(abosoluteFilePath, newFileContents.ToString());
            }
        }
    }
}

#endif
