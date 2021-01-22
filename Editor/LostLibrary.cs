//-----------------------------------------------------------------------
// <copyright file="LostLibrary.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.Addressables;
    using Lost.AppConfig;
    using Lost.PlayFab;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class LostLibrary
    {
        private static readonly string EditorAppConfigBuildSettingsId = "com.lostsignal.appconfig";
        private static readonly string ReleasesEditorBuildSettingsId = "com.lostsignal.releases";
        private static readonly string GameServerEditorBuildSettingsId = "com.lostsignal.gameserver";
        private static readonly string AzureFunctionsEditorBuildSettingsId = "com.lostsignal.azurefunctions";

        private static readonly string LostLibraryAssetsPath = "Assets/Editor/com.lostsignal.lostlibrary";
        private static readonly string AppConfigsAssetName = "AppConfigs.asset";
        private static readonly string ReleasesAssetName = "Releases.asset";
        private static readonly string GameServerAssetName = "GameServerGenerator.asset";
        private static readonly string AzureFunctionsAssetName = "AzureFunctionsGenerator.asset";

        static LostLibrary()
        {
            // Making sure all default assets are created
            EditorApplication.delayCall += () =>
            {
                var relases = Releases;
                var appConfigs = AppConfigs;
                var gameServerGenerator = GameServerProjectGenerator;
                var azureFunctionsGenerator = AzureFunctionsProjectGenerator;
            };
        }

        public static Releases Releases
        {
            get
            {
                if (EditorBuildSettings.TryGetConfigObject(ReleasesEditorBuildSettingsId, out Releases releases) == false || !releases)
                {
                    releases = CreateReleases();
                    EditorBuildSettings.AddConfigObject(ReleasesEditorBuildSettingsId, releases, true);
                }

                return releases;
            }
        }

        public static EditorAppConfig AppConfigs
        {
            get
            {
                if (EditorBuildSettings.TryGetConfigObject(EditorAppConfigBuildSettingsId, out EditorAppConfig editorAppConfig) == false || !editorAppConfig)
                {
                    editorAppConfig = CreateEditorAppConfig();
                    EditorBuildSettings.AddConfigObject(EditorAppConfigBuildSettingsId, editorAppConfig, true);
                    EditorAppConfigFileBuidler.GenerateAppConfigsFile();
                }

                if (editorAppConfig)
                {
                    if (editorAppConfig.AppConfigs == null || editorAppConfig.AppConfigs.Count == 0)
                    {
                        Debug.LogError("AppConfigs doesn't have any valid configs in it's list.");
                    }
                    else if (editorAppConfig.DefaultAppConfig == null)
                    {
                        Debug.LogError("AppConfigs doesn't have a valid default config.");
                    }
                    else if (editorAppConfig.RootAppConfig == null)
                    {
                        Debug.LogError("EditorAppConfig doesn't have a valid root config.");
                    }
                }

                return editorAppConfig;
            }
        }

        public static GameServerProjectGenerator GameServerProjectGenerator
        {
            get
            {
                if (EditorBuildSettings.TryGetConfigObject(GameServerEditorBuildSettingsId, out GameServerProjectGenerator gameServerGenerator) == false || !gameServerGenerator)
                {
                    gameServerGenerator = CreateGameServerProjectGenerator();
                    EditorBuildSettings.AddConfigObject(GameServerEditorBuildSettingsId, gameServerGenerator, true);
                }

                return gameServerGenerator;
            }
        }

        public static AzureFunctionsProjectGenerator AzureFunctionsProjectGenerator
        {
            get
            {
                if (EditorBuildSettings.TryGetConfigObject(AzureFunctionsEditorBuildSettingsId, out AzureFunctionsProjectGenerator azureFunctionsGenerator) == false || !azureFunctionsGenerator)
                {
                    azureFunctionsGenerator = CreateAzureFunctionsProjectGenerator();
                    EditorBuildSettings.AddConfigObject(AzureFunctionsEditorBuildSettingsId, azureFunctionsGenerator, true);
                }

                return azureFunctionsGenerator;
            }
        }

        private static Releases CreateReleases()
        {
            string releasesAssetPath = GetAssetPath(ReleasesAssetName);

            Releases releasesObject;

            if (File.Exists(releasesAssetPath) == false)
            {
                releasesObject = ScriptableObject.CreateInstance<Releases>();
                CreateAsset(releasesObject, releasesAssetPath);
            }
            else
            {
                releasesObject = AssetDatabase.LoadAssetAtPath<Releases>(releasesAssetPath);
            }

            return releasesObject;
        }

        private static EditorAppConfig CreateEditorAppConfig()
        {
            string editorAppConfigAssetPath = GetAssetPath(AppConfigsAssetName);

            EditorAppConfig editorAppConfig;

            if (File.Exists(editorAppConfigAssetPath) == false)
            {
                var rootConfig = new AppConfig.AppConfig();
                rootConfig.Name = "Root";
                AddSetting<BundleIdentifierSetting>(rootConfig);
                AddSetting<BuildPlayerContentSettings>(rootConfig);
                AddSetting<GeneralAppSettings>(rootConfig);
                AddSetting<ReleasesSettings>(rootConfig);
                AddSetting<OverrideTemplatesSettings>(rootConfig);
                AddSetting<CloudBuildSetBuildNumber>(rootConfig);

                var devConfig = new AppConfig.AppConfig();
                devConfig.Name = "Dev";
                devConfig.IsDefault = true;
                devConfig.ParentId = rootConfig.Id;
                AddSetting<DevelopmentBuildSetting>(devConfig).IsDevelopmentBuild = true;
                AddSetting<PlayFabSettings>(devConfig).IsDevelopmentEnvironment = true;

                var liveConfig = new AppConfig.AppConfig();
                liveConfig.Name = "Live";
                liveConfig.ParentId = rootConfig.Id;
                AddSetting<DevelopmentBuildSetting>(liveConfig).IsDevelopmentBuild = false;
                AddSetting<PlayFabSettings>(liveConfig).IsDevelopmentEnvironment = false;

                // Creating the AppConfigs scriptable object
                editorAppConfig = ScriptableObject.CreateInstance<EditorAppConfig>();
                editorAppConfig.AppConfigs.Add(rootConfig);
                editorAppConfig.AppConfigs.Add(devConfig);
                editorAppConfig.AppConfigs.Add(liveConfig);

                CreateAsset(editorAppConfig, editorAppConfigAssetPath);
            }
            else
            {
                editorAppConfig = AssetDatabase.LoadAssetAtPath<EditorAppConfig>(editorAppConfigAssetPath);
            }

            return editorAppConfig;

            T AddSetting<T>(AppConfig.AppConfig config) where T : AppConfigSettings, new()
            {
                var newSettings = new T();
                config.Settings.Add(newSettings);
                return newSettings;
            }
        }

        private static GameServerProjectGenerator CreateGameServerProjectGenerator()
        {
            string gameServerAssetPath = GetAssetPath(GameServerAssetName);
            GameServerProjectGenerator gameServerGeneratorObject;

            if (File.Exists(gameServerAssetPath) == false)
            {
                gameServerGeneratorObject = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameServerProjectGenerator>(AssetDatabase.GUIDToAssetPath("a8857427ec41cb94985737f62f7e6383")));
                CreateAsset(gameServerGeneratorObject, gameServerAssetPath);
            }
            else
            {
                gameServerGeneratorObject = AssetDatabase.LoadAssetAtPath<GameServerProjectGenerator>(gameServerAssetPath);
            }

            return gameServerGeneratorObject;
        }

        private static AzureFunctionsProjectGenerator CreateAzureFunctionsProjectGenerator()
        {
            string azureFunctionsAssetPath = GetAssetPath(AzureFunctionsAssetName);
            AzureFunctionsProjectGenerator azureFunctionsGeneratorObject;

            if (File.Exists(azureFunctionsAssetPath) == false)
            {
                azureFunctionsGeneratorObject = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<AzureFunctionsProjectGenerator>(AssetDatabase.GUIDToAssetPath("264a336d161a54948b392550765de177")));
                CreateAsset(azureFunctionsGeneratorObject, azureFunctionsAssetPath);
            }
            else
            {
                azureFunctionsGeneratorObject = AssetDatabase.LoadAssetAtPath<AzureFunctionsProjectGenerator>(azureFunctionsAssetPath);
            }

            return azureFunctionsGeneratorObject;
        }

        private static void CreateAsset(UnityEngine.Object asset, string path)
        {
            AssetDatabase.CreateAsset(asset, path);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        private static string GetAssetPath(string assetName)
        {
            // Making sure EditorAppConfig path exists
            string assetPath = Path.Combine(LostLibraryAssetsPath, assetName);
            string assetDirectory = Path.GetDirectoryName(assetPath);

            if (Directory.Exists(assetDirectory) == false)
            {
                Directory.CreateDirectory(assetDirectory);
            }

            return assetPath;
        }
    }
}
