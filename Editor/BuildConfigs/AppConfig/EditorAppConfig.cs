//-----------------------------------------------------------------------
// <copyright file="EditorAppConfig.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Lost.Addressables;
    using PlayFab;
    using UnityEditor;
    using UnityEngine;

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
    public class EditorAppConfig : ScriptableObject
    {
        private static readonly string CSharpFileName = "AppConfigsMenuItems.cs";

#pragma warning disable 0649
        [SerializeField] private List<AppConfig> appConfigs = new List<AppConfig>();
#pragma warning restore 0649

        [NonSerialized] private AppConfig activeAppConfig;

        static EditorAppConfig()
        {
            EditorApplication.delayCall += InitializeOnLoad;
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
            BuildPlayerWindow.RegisterGetBuildPlayerOptionsHandler(BuildPlayerOptionsHandler);
        }

        public List<AppConfig> AppConfigs => this.appConfigs;

        public AppConfig RootAppConfig => this.appConfigs.FirstOrDefault(x => string.IsNullOrEmpty(x.ParentId));

        public AppConfig DefaultAppConfig => this.appConfigs.Where(x => x.IsDefault).FirstOrDefault();

        public static string AppConfigScriptPath
        {
            get { return Path.Combine(Path.GetDirectoryName(LostLibrary.AppConfigs.GetPath()), CSharpFileName); }
        }

        public static AppConfig ActiveAppConfig
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

                var instance = LostLibrary.AppConfigs;

                if (instance.activeAppConfig != null)
                {
                    // Do nothing, already set
                }
                else if (RuntimeBuildConfig.Instance != null && string.IsNullOrEmpty(RuntimeBuildConfig.Instance.BuildConfigGuid) == false)
                {
                    foreach (var config in instance.AppConfigs)
                    {
                        if (config.Id == RuntimeBuildConfig.Instance.BuildConfigGuid)
                        {
                            instance.activeAppConfig = config;
                            break;
                        }
                    }
                }
                else
                {
                    instance.activeAppConfig = instance.DefaultAppConfig;
                    WriteRuntimeConfigFile();
                }

                return instance.activeAppConfig;
            }
        }

        private static void WriteRuntimeConfigFile()
        {
            RuntimeBuildConfig.Reset();

            AppConfig activeConfig = ActiveAppConfig;

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
            string configJson = JsonUtility.ToJson(runtimeConfig);

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
            foreach (var config in LostLibrary.AppConfigs.AppConfigs)
            {
                if (config.Id == guid)
                {
                    LostLibrary.AppConfigs.activeAppConfig = config;
                    EditorAppConfig.InitializeOnLoad();

                    if (Platform.IsUnityCloudBuild)
                    {
                        var activeConfig = EditorAppConfig.ActiveAppConfig;

                        // Telling all the app configs that a unity cloud build is about to begin
                        foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
                        {
                            settings.OnUnityCloudBuildInitiated(activeConfig);
                        }
                    }

                    break;
                }
            }
        }

        public static bool IsActiveConfig(string guid)
        {
            return guid == EditorAppConfig.ActiveAppConfig.Id;
        }

        public static IEnumerable<AppConfigSettings> GetActiveConfigSettings()
        {
            var activeConfig = ActiveAppConfig;

            if (activeConfig == null)
            {
                yield break;
            }

            foreach (var type in TypeUtil.GetAllTypesOf<AppConfigSettings>())
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
            var activeConfig = EditorAppConfig.ActiveAppConfig;

            // Telling all the app configs that a user has started a build
            foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
            {
                settings.OnUserBuildInitiated(activeConfig);
            }

            // Telling all the app configs to update player options
            foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
            {
                options = settings.ChangeBuildPlayerOptions(activeConfig, options);
            }

            BuildPipeline.BuildPlayer(options);
        }

        [MenuItem("Tools/Lost/Work In Progress/InitializeOnLoad and OnAppSetingsChanged")]
        private static void InitializeOnLoad()
        {
            if (LostLibrary.AppConfigs == null)
            {
                return;
            }

            EditorApplication.playModeStateChanged += PlayModeStateChanged;

            // Recording defines before we possibly alter them
            List<string> definesBefore = new List<string>();
            BuildTargetGroupUtil.GetValid().ForEach(x => definesBefore.Add(PlayerSettings.GetScriptingDefineSymbolsForGroup(x)));

            EditorAppConfigDefinesHelper.UpdateProjectDefines();

            var activeConfig = ActiveAppConfig;

            foreach (var settings in GetActiveConfigSettings())
            {
                settings.InitializeOnLoad(activeConfig);
            }

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

        private static void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (EditorApplication.isPlaying == false && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                WriteRuntimeConfigFile();

                var activeConfig = EditorAppConfig.ActiveAppConfig;

                foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
                {
                    settings.OnEnteringPlayMode(activeConfig);
                }
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                var activeConfig = EditorAppConfig.ActiveAppConfig;

                foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
                {
                    settings.OnExitingPlayMode(activeConfig);
                }
            }
        }
    }
}
