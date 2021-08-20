//-----------------------------------------------------------------------
// <copyright file="EditorBuildConfigs.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Serialization;

    // TODO [bgish]: Must make sure all AppConfigs are in the appConfigs list
    // TODO [bgish]: Must make sure there is only one root config (and make sure the rootConfig is set to that) - Possibly rename to Default config
    // TODO [bgish]: Need to make sure all configs specify a ShortName (with no spaces or special characters)
    // TODO [bgish]: Need to make sure no configs (or at least config heiracrhy) has the same name (not sure how to do this)
    //
    // Generate file for switch what's the active config
    // Shoudl add the runtime json to the .p4ignore and .gitignore
    //
    // TODO [bgish]: Turn the folder and script path strings to DefaultAssets, then make a
    //               property attribute that will make sure it's a file and a .cs file
    //               Check out the LazyAsset stuff, I think I already do something similar there.
    //               Like on hover don't accept it.

    [InitializeOnLoad]
    public class EditorBuildConfigs : ScriptableObject
    {
        private static readonly string CSharpFileName = "BuildConfigsMenuItems.cs";

#pragma warning disable 0649
        [FormerlySerializedAs("appConfigs")]
        [SerializeField] private List<BuildConfig> buildConfigs = new List<BuildConfig>();
#pragma warning restore 0649

        [NonSerialized] private BuildConfig activeBuildConfig;

        static EditorBuildConfigs()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
            BuildPlayerWindow.RegisterGetBuildPlayerOptionsHandler(BuildPlayerOptionsHandler);
        }

        public List<BuildConfig> BuildConfigs => this.buildConfigs;

        public BuildConfig RootBuildConfig => this.buildConfigs.FirstOrDefault(x => string.IsNullOrEmpty(x.ParentId));

        public BuildConfig DefaultBuildConfig => this.buildConfigs.Where(x => x.IsDefault).FirstOrDefault();

        public static string BuildConfigsScriptPath
        {
            get { return Path.Combine(Path.GetDirectoryName(LostLibrary.BuildConfigs.GetPath()), CSharpFileName); }
        }

        public static BuildConfig ActiveBuildConfig
        {
            get
            {
                // Special case if someone has deleted the last selected app config
                if (RuntimeBuildConfig.Instance != null &&
                    string.IsNullOrEmpty(RuntimeBuildConfig.Instance.BuildConfigGuid) == false &&
                    string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(RuntimeBuildConfig.Instance.BuildConfigGuid)))
                {
                    RuntimeBuildConfig.Instance.BuildConfigGuid = null;
                }

                var instance = LostLibrary.BuildConfigs;

                if (instance.activeBuildConfig != null)
                {
                    // Do nothing, already set
                }
                else if (RuntimeBuildConfig.Instance != null && string.IsNullOrEmpty(RuntimeBuildConfig.Instance.BuildConfigGuid) == false)
                {
                    foreach (var config in instance.BuildConfigs)
                    {
                        if (config.Id == RuntimeBuildConfig.Instance.BuildConfigGuid)
                        {
                            instance.activeBuildConfig = config;
                            break;
                        }
                    }
                }
                else
                {
                    instance.activeBuildConfig = instance.DefaultBuildConfig;
                    WriteRuntimeConfigFile();
                }

                return instance.activeBuildConfig;
            }
        }

        public static T GetActiveSettings<T>() where T : BuildConfigSettings
        {
            var activeConfig = ActiveBuildConfig;

            if (activeConfig != null)
            {
                return activeConfig.GetSettings<T>();
            }

            return null;
        }

        private static void WriteRuntimeConfigFile()
        {
            RuntimeBuildConfig.Reset();

            BuildConfig activeConfig = ActiveBuildConfig;

            if (activeConfig == null)
            {
                return;
            }

            // Collecting all the runtime config values
            var runtimeConfigValues = new Dictionary<string, string>();

            foreach (var settings in GetActiveConfigSettings())
            {
                settings.GetRuntimeConfigSettings(activeConfig, runtimeConfigValues);
            }

            // Generating the runtime config object and serializing to json
            var runtimeConfig = new RuntimeBuildConfig(activeConfig.Id, activeConfig.SafeName, runtimeConfigValues);
            string configJson = JsonUtility.ToJson(runtimeConfig, true);

            // Early out if the file file hasn't chenged
            if (File.Exists(RuntimeBuildConfig.FilePath) && File.ReadAllText(RuntimeBuildConfig.FilePath) == configJson)
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(RuntimeBuildConfig.FilePath));
            File.WriteAllText(RuntimeBuildConfig.FilePath, configJson);
            AssetDatabase.ImportAsset(RuntimeBuildConfig.FilePath);
            AssetDatabase.Refresh();
        }

        public static void SetActiveConfig(string guid)
        {
            foreach (var config in LostLibrary.BuildConfigs.BuildConfigs)
            {
                if (config.Id == guid)
                {
                    LostLibrary.BuildConfigs.activeBuildConfig = config;

                    EditorBuildConfigs.OnDomainReload();

                    if (Platform.IsUnityCloudBuild || Application.isBatchMode)
                    {
                        EditorEventsExecutor.ExecuteAttribute<EditorEvents.OnCloudBuildInitiatedAttribute>();
                    }

                    break;
                }
            }
        }

        public static bool IsActiveConfig(string guid)
        {
            return guid == EditorBuildConfigs.ActiveBuildConfig.Id;
        }

        public static IEnumerable<BuildConfigSettings> GetActiveConfigSettings()
        {
            var activeConfig = ActiveBuildConfig;

            if (activeConfig == null)
            {
                yield break;
            }

            foreach (var type in TypeUtil.GetAllTypesOf<BuildConfigSettings>())
            {
                var settings = activeConfig.GetSettings(type);

                if (settings != null)
                {
                    yield return settings;
                }
            }
        }

        private static BuildPlayerOptions BuildPlayerOptionsHandler(BuildPlayerOptions options)
        {
            return BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);
        }

        private static void BuildPlayerHandler(BuildPlayerOptions options)
        {
            var activeConfig = EditorBuildConfigs.ActiveBuildConfig;

            EditorEventsExecutor.ExecuteAttribute<EditorEvents.OnUserBuildInitiatedAttribute>();

            // Telling all the app configs to update player options
            foreach (var settings in EditorBuildConfigs.GetActiveConfigSettings())
            {
                options = settings.ChangeBuildPlayerOptions(activeConfig, options);
            }

            BuildPipeline.BuildPlayer(options);
        }

        [EditorEvents.OnDomainReload]
        [MenuItem("Tools/Lost/Work In Progress/OnDomainReload and OnAppSetingsChanged")]
        private static void OnDomainReload()
        {
            if (LostLibrary.BuildConfigs == null)
            {
                return;
            }

            // Recording defines before we possibly alter them
            List<string> definesBefore = new List<string>();
            BuildTargetGroupUtil.GetValid().ForEach(x => definesBefore.Add(PlayerSettings.GetScriptingDefineSymbolsForGroup(x)));

            EditorBuildConfigDefinesHelper.UpdateProjectDefines();

            // Recording defines after we've possibly altered them
            List<string> definesAfter = new List<string>();
            BuildTargetGroupUtil.GetValid().ForEach(x => definesAfter.Add(PlayerSettings.GetScriptingDefineSymbolsForGroup(x)));

            // checking to see if the scripting defines have changed
            bool forceRecompile = definesBefore.Count != definesAfter.Count;

            if (forceRecompile == false)
            {
                for (int i = 0; i < definesBefore.Count; i++)
                {
                    if (definesBefore[i] != definesAfter[i])
                    {
                        forceRecompile = true;
                        break;
                    }
                }
            }

            if (forceRecompile)
            {
                // TODO [bgish]: Is this neccessary, if so implement
            }

            WriteRuntimeConfigFile();

            // TODO [bgish]: Write out the MenuItems class? (force recompile if new)
        }

        [EditorEvents.OnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            if (LostLibrary.BuildConfigs == null)
            {
                return;
            }

            WriteRuntimeConfigFile();
        }
    }
}
