#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LostLibrary.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.Addressables;
    using Lost.BuildConfig;
    using Lost.PlayFab;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class LostLibrary
    {
        private static readonly string OldEditorAppConfigBuildSettingsId = "com.lostsignal.appconfig";
        private static readonly string EditorBuildConfigsBuildSettingsId = "com.lostsignal.buildconfigs";
        private static readonly string ReleasesEditorBuildSettingsId = "com.lostsignal.releases";
        private static readonly string GameServerEditorBuildSettingsId = "com.lostsignal.gameserver";
        private static readonly string AzureFunctionsEditorBuildSettingsId = "com.lostsignal.azurefunctions";

        private static readonly string LostLibraryAssetsPath = "Assets/Editor/com.lostsignal.lostlibrary";
        private static readonly string BuildConfigsAssetName = "BuildConfigs.asset";
        private static readonly string ReleasesAssetName = "Releases.asset";
        private static readonly string GameServerAssetName = "GameServerGenerator.asset";
        private static readonly string AzureFunctionsAssetName = "AzureFunctionsGenerator.asset";

        static LostLibrary()
        {
            // Making sure all default assets are created
            EditorApplication.delayCall += () =>
            {
                var relases = Releases;
                var buildConfigs = BuildConfigs;
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

        public static EditorBuildConfigs BuildConfigs
        {
            get
            {
                // Moving build configs if was once using the old system
                if (EditorBuildSettings.TryGetConfigObject(OldEditorAppConfigBuildSettingsId, out EditorBuildConfigs editorBuildConfigs))
                {
                    EditorBuildSettings.AddConfigObject(EditorBuildConfigsBuildSettingsId, editorBuildConfigs, false);
                    EditorBuildSettings.RemoveConfigObject(OldEditorAppConfigBuildSettingsId);
                }

                if (EditorBuildSettings.TryGetConfigObject(EditorBuildConfigsBuildSettingsId, out editorBuildConfigs) == false || !editorBuildConfigs)
                {
                    editorBuildConfigs = CreateEditorBuildConfigs();
                    EditorBuildSettings.AddConfigObject(EditorBuildConfigsBuildSettingsId, editorBuildConfigs, true);
                    EditorBuildConfigFileBuidler.GenerateBuildConfigsFile();
                }

                if (editorBuildConfigs)
                {
                    if (editorBuildConfigs.BuildConfigs == null || editorBuildConfigs.BuildConfigs.Count == 0)
                    {
                        Debug.LogError("BuildConfigs doesn't have any valid configs in it's list.");
                    }
                    else if (editorBuildConfigs.DefaultBuildConfig == null)
                    {
                        Debug.LogError("BuildConfigs doesn't have a valid default config.");
                    }
                    else if (editorBuildConfigs.RootBuildConfig == null)
                    {
                        Debug.LogError("EditorBuildConfig doesn't have a valid root config.");
                    }
                }

                return editorBuildConfigs;
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

        private static EditorBuildConfigs CreateEditorBuildConfigs()
        {
            string editorBuildConfigAssetPath = GetAssetPath(BuildConfigsAssetName);

            EditorBuildConfigs editorBuildConfigs;

            if (File.Exists(editorBuildConfigAssetPath) == false)
            {
                var rootConfig = new BuildConfig.BuildConfig();
                rootConfig.Name = "Root";
                AddSetting<BundleIdentifierSetting>(rootConfig);
                AddSetting<BootloaderSettings>(rootConfig);
                AddSetting<BuildPlayerContentSettings>(rootConfig);
                AddSetting<CloudBuildSetBuildNumber>(rootConfig);

                var devConfig = new BuildConfig.BuildConfig();
                devConfig.Name = "Dev";
                devConfig.IsDefault = true;
                devConfig.ParentId = rootConfig.Id;
                AddSetting<DevelopmentBuildSetting>(devConfig).IsDevelopmentBuild = true;
                AddSetting<PlayFabSettings>(devConfig).IsDevelopmentEnvironment = true;

                var liveConfig = new BuildConfig.BuildConfig();
                liveConfig.Name = "Live";
                liveConfig.ParentId = rootConfig.Id;
                AddSetting<DevelopmentBuildSetting>(liveConfig).IsDevelopmentBuild = false;
                AddSetting<PlayFabSettings>(liveConfig).IsDevelopmentEnvironment = false;

                // Creating the AppConfigs scriptable object
                editorBuildConfigs = ScriptableObject.CreateInstance<EditorBuildConfigs>();
                editorBuildConfigs.BuildConfigs.Add(rootConfig);
                editorBuildConfigs.BuildConfigs.Add(devConfig);
                editorBuildConfigs.BuildConfigs.Add(liveConfig);

                CreateAsset(editorBuildConfigs, editorBuildConfigAssetPath);
            }
            else
            {
                editorBuildConfigs = AssetDatabase.LoadAssetAtPath<EditorBuildConfigs>(editorBuildConfigAssetPath);
            }

            return editorBuildConfigs;

            T AddSetting<T>(BuildConfig.BuildConfig config) where T : BuildConfigSettings, new()
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

        //// TODO [bgish]: Need to make a button somewhere in Bootloader Settings for creating these assets.  Should also
        ////               prompt user for the location instead of hard coding it.
        //// private static void CreateBootloaderAndManagers()
        //// {
        ////     var bootloader = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<Bootloader>(AssetDatabase.GUIDToAssetPath("e64035672fd9d3848956e0518ca53808")));
        ////     CreateAsset(bootloader, $"Assets/Resources/Bootloader.prefab");
        ////
        ////     var managers = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath("ebe6a31cc5c4ac74ab8dae1375be0b50")));
        ////     CreateAsset(managers, $"Assets/Resources/Managers.prefab");
        //// }
    }
}
