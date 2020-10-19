//-----------------------------------------------------------------------
// <copyright file="EditorAppConfig.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        private static readonly string EditorBuildSettingsName = "com.lostsignal.appconfig";
        private static readonly string AppConfigPath = "Assets/Editor/com.lostsignal.lostlibrary";
        private static readonly string EditorAppConfigAssetName = "EditorAppConfig.asset";
        private static readonly string CSharpFileName = "EditorAppConfigs.cs";
        private static readonly string ConfigsPath = AppConfigPath + "/AppConfigs";
        private static readonly string RootConfigName = "Root.asset";
        private static readonly string DevConfigName = "Dev.asset";
        private static readonly string LiveConfigName = "Live.asset";

#pragma warning disable 0649
        [SerializeField] private List<AppConfig> appConfigs;
        [SerializeField] private AppConfig defaultAppConfig;
        [SerializeField] private AppConfig rootConfig;
#pragma warning restore 0649

        [NonSerialized] private AppConfig activeAppConfig;

        static EditorAppConfig()
        {
            EditorApplication.delayCall += InitializeOnLoad;
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
            BuildPlayerWindow.RegisterGetBuildPlayerOptionsHandler(BuildPlayerOptionsHandler);

            // Making sure all the initial config files exist
            EditorApplication.delayCall += () =>
            {
                if (Instance == null)
                {
                    CreateAppConfigs();
                }
            };
        }

        public static List<AppConfig> AppConfigs => Instance?.appConfigs;

        public static AppConfig RootAppConfig => Instance?.rootConfig;

        public static string AppConfigScriptPath
        {
            get { return Instance == null ? null : Path.Combine(Path.GetDirectoryName(Instance.GetPath()), CSharpFileName); }
        }

        public static AppConfig ActiveAppConfig
        {
            get
            {
                // Special case if someone has deleted the last selected app config
                if (string.IsNullOrEmpty(RuntimeAppConfig.Instance.AppConfigGuid) == false &&
                    string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(RuntimeAppConfig.Instance.AppConfigGuid)))
                {
                    RuntimeAppConfig.Instance.AppConfigGuid = null;
                }

                if (Instance == null)
                {
                    return null;
                }
                else if (Instance.activeAppConfig)
                {
                    // Do nothing, already set
                }
                else if (string.IsNullOrEmpty(RuntimeAppConfig.Instance.AppConfigGuid) == false)
                {
                    foreach (var config in Instance.appConfigs)
                    {
                        if (config.GetGuid() == RuntimeAppConfig.Instance.AppConfigGuid)
                        {
                            Instance.activeAppConfig = config;
                            break;
                        }
                    }
                }
                else
                {
                    Instance.activeAppConfig = Instance.defaultAppConfig;
                    WriteRuntimeConfigFile();
                }

                return Instance.activeAppConfig;
            }
        }

        private static EditorAppConfig Instance
        {
            get
            {
                EditorAppConfig editorAppConfig = null;
                EditorBuildSettings.TryGetConfigObject(EditorBuildSettingsName, out editorAppConfig);

                if (editorAppConfig != null)
                {
                    if (editorAppConfig.defaultAppConfig == null)
                    {
                        Debug.LogError("EditorAppConfig doesn't have a default config.");
                        return null;
                    }

                    if (editorAppConfig.appConfigs == null || editorAppConfig.appConfigs.Count == 0)
                    {
                        Debug.LogError("EditorAppConfig doesn't have any app configs in it's list.");
                        return null;
                    }

                    if (editorAppConfig.appConfigs.Contains(editorAppConfig.defaultAppConfig) == false)
                    {
                        Debug.LogError("EditorAppConfig's configs list doesn't contain the default app config.");
                        return null;
                    }
                }

                return editorAppConfig;
            }
        }

        public static void CreateAppConfigs()
        {
            // Making sure EditorAppConfig path exists
            string editorAppConfigAssetPath = Path.Combine(AppConfigPath, EditorAppConfigAssetName);
            string editorAppConfigDirectory = Path.GetDirectoryName(editorAppConfigAssetPath);

            if (Directory.Exists(editorAppConfigDirectory) == false)
            {
                Directory.CreateDirectory(editorAppConfigDirectory);
            }

            // Making sure Root Config path exists
            string rootConfigAssetPath = Path.Combine(ConfigsPath, RootConfigName);
            string devConfigAssetPath = Path.Combine(ConfigsPath, DevConfigName);
            string liveConfigAssetPath = Path.Combine(ConfigsPath, LiveConfigName);

            if (Directory.Exists(ConfigsPath) == false)
            {
                Directory.CreateDirectory(ConfigsPath);
            }

            EditorAppConfig editorAppConfig = null;

            if (File.Exists(editorAppConfigAssetPath) == false)
            {
                var rootConfig = ScriptableObject.CreateInstance<AppConfig>();
                AssetDatabase.CreateAsset(rootConfig, rootConfigAssetPath);
                rootConfig = AssetDatabase.LoadAssetAtPath<AppConfig>(rootConfigAssetPath);
                AddSetting<BundleIdentifierSetting>(rootConfig);
                AddSetting<BuildPlayerContentSettings>(rootConfig);
                AddSetting<ReleasesSettings>(rootConfig);
                AddSetting<GeneralAppSettings>(rootConfig);
                AddSetting<OverrideTemplatesSettings>(rootConfig);
                AddSetting<PerforceSettings>(rootConfig);
                AddSetting<AndroidKeystoreSettings>(rootConfig);
                AddSetting<LostDefineSettings>(rootConfig);

                var devConfig = ScriptableObject.CreateInstance<AppConfig>();
                AssetDatabase.CreateAsset(devConfig, devConfigAssetPath);
                devConfig = AssetDatabase.LoadAssetAtPath<AppConfig>(devConfigAssetPath);
                devConfig.Parent = rootConfig;
                AddSetting<DevelopmentBuildSetting>(devConfig).IsDevelopmentBuild = true;
                AddSetting<PlayFabSettings>(devConfig).IsDevelopmentEnvironment = true;

                var liveConfig = ScriptableObject.CreateInstance<AppConfig>();
                AssetDatabase.CreateAsset(liveConfig, liveConfigAssetPath);
                liveConfig = AssetDatabase.LoadAssetAtPath<AppConfig>(liveConfigAssetPath);
                liveConfig.Parent = rootConfig;
                AddSetting<DevelopmentBuildSetting>(liveConfig).IsDevelopmentBuild = false;
                AddSetting<PlayFabSettings>(liveConfig).IsDevelopmentEnvironment = false;

                editorAppConfig = ScriptableObject.CreateInstance<EditorAppConfig>();
                editorAppConfig.rootConfig = rootConfig;
                editorAppConfig.defaultAppConfig = devConfig;
                editorAppConfig.appConfigs = new List<AppConfig> { rootConfig, devConfig, liveConfig };
                AssetDatabase.CreateAsset(editorAppConfig, editorAppConfigAssetPath);
                EditorUtility.SetDirty(editorAppConfig);
                EditorUtility.SetDirty(rootConfig);
                EditorUtility.SetDirty(devConfig);
                EditorUtility.SetDirty(liveConfig);

                AssetDatabase.SaveAssets();
            }
            else
            {
                editorAppConfig = AssetDatabase.LoadAssetAtPath<EditorAppConfig>(editorAppConfigAssetPath);
            }

            EditorBuildSettings.AddConfigObject(EditorBuildSettingsName, editorAppConfig, true);
            EditorAppConfigFileBuidler.GenerateAppConfigsFile();

            T AddSetting<T>(AppConfig config) where T : AppConfigSettings
            {
                var newSettings = ScriptableObject.CreateInstance<T>();
                newSettings.name = typeof(T).Name;
                config.Settings.Add(newSettings);

                string path = config.GetPath();
                AssetDatabase.AddObjectToAsset(newSettings, path);
                AssetDatabase.ImportAsset(path);

                return newSettings;
            }
        }

        private static void WriteRuntimeConfigFile()
        {
            RuntimeAppConfig.Reset();

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
            var runtimeConfig = new RuntimeAppConfig(activeConfig.GetGuid(), activeConfig.SafeName, runtimeConfigValues);
            string configJson = JsonUtility.ToJson(runtimeConfig);

            // Early out if the file file hasn't chenged
            if (File.Exists(RuntimeAppConfig.FilePath) && File.ReadAllText(RuntimeAppConfig.FilePath) == configJson)
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(RuntimeAppConfig.FilePath));
            File.WriteAllText(RuntimeAppConfig.FilePath, configJson);
            AssetDatabase.ImportAsset(RuntimeAppConfig.FilePath);
            AssetDatabase.Refresh();
        }

        public static void SetActiveConfig(string guid)
        {
            if (Instance == null)
            {
                return;
            }

            foreach (var config in Instance.appConfigs)
            {
                if (config.GetGuid() == guid)
                {
                    Instance.activeAppConfig = config;
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
            return guid == EditorAppConfig.ActiveAppConfig.GetGuid();
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
            if (Instance == null)
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
