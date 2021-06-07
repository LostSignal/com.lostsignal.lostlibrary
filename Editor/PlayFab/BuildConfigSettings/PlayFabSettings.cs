//-----------------------------------------------------------------------
// <copyright file="PlayFabSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using System.Collections.Generic;
    using System.IO;
    using Lost.BuildConfig;
    using UnityEngine;

    [BuildConfigSettingsOrder(500)]
    public class PlayFabSettings : BuildConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private string titleId;
        [SerializeField] private string secretKey;
        [SerializeField] private bool isDevelopmentEnvironment;

        [Header("Azure Functions")]
        [SerializeField] private string functionsSite;
        [SerializeField] private string functionsHostKey;
        [SerializeField] private string ablyServerKey;
        [SerializeField] private string redisConnectionString;
        [SerializeField] private string cosmosDbConnectionString;
#pragma warning restore 0649

        public override string DisplayName => "PlayFab";

        public override bool IsInline => false;

        public string TitleId => this.titleId;

        public string SecretKey => this.secretKey;

        public bool IsDevelopmentEnvironment
        {
            get => this.isDevelopmentEnvironment;
            set => this.isDevelopmentEnvironment = value;
        }

        public string FunctionsSite => this.functionsSite;

        public string FunctionsHostKey => this.functionsHostKey;

        public string CosmosDbConnectionString => this.cosmosDbConnectionString;

        public string RedisConnectionString => this.redisConnectionString;

        public string AblySererKey => this.ablyServerKey;

        public override void GetRuntimeConfigSettings(Lost.BuildConfig.BuildConfig buildConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var playFabSettings = buildConfig.GetSettings<PlayFabSettings>();

            if (playFabSettings == null)
            {
                return;
            }

            runtimeConfigSettings.Add(PlayFabConfigExtensions.TitleId, playFabSettings.titleId);
        }

        [EditorEvents.OnPostprocessBuild]
        private static void OnPostprocessBuild()
        {
            var playFabSettings = EditorBuildConfigs.GetActiveSettings<PlayFabSettings>();

            if (playFabSettings == null || LostLibrary.AzureFunctionsProjectGenerator == null)
            {
                return;
            }

            LostLibrary.AzureFunctionsProjectGenerator.UploadFunctionsToPlayFab();

            //// TODO [bgish]: If Platform.IsUnityCloudBuild, check if Upload was successful and fail the build if it wasn't
        }

        [EditorEvents.OnDomainReload]
        private static void OnDomainReload()
        {
            var playfabSettings = EditorBuildConfigs.GetActiveSettings<PlayFabSettings>();

            if (playfabSettings == null)
            {
                return;
            }

            global::PlayFab.PlayFabSettings.staticSettings.TitleId = playfabSettings.titleId;
            global::PlayFab.PlayFabSettings.staticSettings.DeveloperSecretKey = playfabSettings.secretKey;
            
            if (LostLibrary.AzureFunctionsProjectGenerator != null)
            {
                GenerateLaunchSettingsForAzureFunctionsProject(playfabSettings);
            }

            if (LostLibrary.GameServerProjectGenerator != null)
            {
                GenerateLaunchSettingsForGameServerProject(playfabSettings);
            }
        }

        private static void GenerateLaunchSettingsForAzureFunctionsProject(PlayFabSettings playfabSettings)
        {
            string launchSettings = BetterStringBuilder.New()
                .AppendLine("{")
                .AppendLine("  \"profiles\": {")
                .AppendLine("    \"" + LostLibrary.AzureFunctionsProjectGenerator.ProjectName + "\": {")
                .AppendLine("      \"commandName\": \"Project\",")
                .AppendLine("      \"environmentVariables\": {")
                .AppendLine($"        \"ABLY_SERVER_KEY\": \"{playfabSettings.ablyServerKey}\",")
                .AppendLine($"        \"COSMOS_DB_CONNECTION_STRING\": \"{playfabSettings.cosmosDbConnectionString}\",")
                .AppendLine($"        \"DEVELOPMENT\": \"{playfabSettings.isDevelopmentEnvironment.ToString().ToUpperInvariant()}\",")
                .AppendLine($"        \"PF_TITLE_ID\": \"{playfabSettings.titleId}\",")
                .AppendLine($"        \"PF_SECRET_KEY\": \"{playfabSettings.secretKey}\",")
                .AppendLine($"        \"REDIS_CONNECTION_STRING\": \"{playfabSettings.redisConnectionString}\"")
                .AppendLine("      }")
                .AppendLine("    }")
                .AppendLine("  }")
                .AppendLine("}")
                .ToString();

            string projectDirectory = Path.GetDirectoryName(LostLibrary.AzureFunctionsProjectGenerator.CsProjFilePath);
            string propertiesDirectory = Path.Combine(projectDirectory, "Properties");
            string launchSettingsPath = Path.Combine(propertiesDirectory, "launchSettings.json");

            if (File.Exists(LostLibrary.AzureFunctionsProjectGenerator.CsProjFilePath) == false)
            {
                return;
            }

            if (Directory.Exists(propertiesDirectory) == false)
            {
                Directory.CreateDirectory(propertiesDirectory);
            }

            if (File.Exists(launchSettingsPath) == false || File.ReadAllText(launchSettingsPath) != launchSettings)
            {
                File.WriteAllText(launchSettingsPath, launchSettings);
            }
        }

        private static void GenerateLaunchSettingsForGameServerProject(PlayFabSettings playfabSettings)
        {
            string launchSettings = BetterStringBuilder.New()
                .AppendLine("{")
                .AppendLine("  \"profiles\": {")
                .AppendLine("    \"" + LostLibrary.GameServerProjectGenerator.ProjectName + "\": {")
                .AppendLine("      \"commandName\": \"Project\",")
                .AppendLine("      \"environmentVariables\": {")
                .AppendLine($"        \"PF_TITLE_ID\": \"{playfabSettings.titleId}\",")
                .AppendLine($"        \"PF_SECRET_KEY\": \"{playfabSettings.secretKey}\",")
                .AppendLine("      }")
                .AppendLine("    }")
                .AppendLine("  }")
                .AppendLine("}")
                .ToString();

            string projectDirectory = Path.GetDirectoryName(LostLibrary.GameServerProjectGenerator.CsProjFilePath);
            string propertiesDirectory = Path.Combine(projectDirectory, "Properties");
            string launchSettingsPath = Path.Combine(propertiesDirectory, "launchSettings.json");

            if (File.Exists(LostLibrary.GameServerProjectGenerator.CsProjFilePath) == false)
            {
                return;
            }

            if (Directory.Exists(propertiesDirectory) == false)
            {
                Directory.CreateDirectory(propertiesDirectory);
            }

            if (File.Exists(launchSettingsPath) == false || File.ReadAllText(launchSettingsPath) != launchSettings)
            {
                File.WriteAllText(launchSettingsPath, launchSettings);
            }
        }
    }
}
