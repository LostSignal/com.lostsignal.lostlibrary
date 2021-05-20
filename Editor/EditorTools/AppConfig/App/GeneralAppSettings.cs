//-----------------------------------------------------------------------
// <copyright file="GeneralAppSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.BuildConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(20)]
    public class GeneralAppSettings : AppConfigSettings
    {
        public enum LineEndings
        {
            Unix,
            Windows,
        }

#pragma warning disable 0649
        [SerializeField] private string appVersion;
        [SerializeField] private string productName;
        [SerializeField] private string companyName;
        [SerializeField] private string rootNamespace;
        [SerializeField] private LineEndings lineEndings;
        [SerializeField] private SerializationMode serializationMode = SerializationMode.ForceText;
#pragma warning restore 0649

        public override string DisplayName => "General App Settings";

        public override bool IsInline => false;

        public override void InitializeOnLoad(BuildConfig.AppConfig buildConfig)
        {
            var settings = buildConfig.GetSettings<GeneralAppSettings>();

            if (settings == null)
            {
                return;
            }

            this.GetInitialValues(settings);

            if (string.IsNullOrEmpty(settings.appVersion) == false && PlayerSettings.bundleVersion != settings.appVersion)
            {
                PlayerSettings.bundleVersion = settings.appVersion;
            }

            if (string.IsNullOrEmpty(settings.productName) == false && PlayerSettings.productName != settings.productName)
            {
                PlayerSettings.productName = settings.productName;
            }

            if (string.IsNullOrEmpty(settings.companyName) == false && PlayerSettings.companyName != settings.companyName)
            {
                PlayerSettings.companyName = settings.companyName;
            }

            if (string.IsNullOrEmpty(settings.rootNamespace) == false && EditorSettings.projectGenerationRootNamespace != settings.rootNamespace)
            {
                EditorSettings.projectGenerationRootNamespace = settings.rootNamespace;
            }

            if (EditorSettings.lineEndingsForNewScripts != this.Convert(settings.lineEndings))
            {
                EditorSettings.lineEndingsForNewScripts = this.Convert(settings.lineEndings);
            }

            if (EditorSettings.serializationMode != settings.serializationMode)
            {
                EditorSettings.serializationMode = settings.serializationMode;
            }
        }

        private void GetInitialValues(GeneralAppSettings settings)
        {
            if (string.IsNullOrEmpty(settings.appVersion))
            {
                settings.appVersion = PlayerSettings.bundleVersion;
            }

            if (string.IsNullOrEmpty(settings.productName))
            {
                settings.productName = PlayerSettings.productName;
            }

            if (string.IsNullOrEmpty(settings.companyName))
            {
                settings.companyName = PlayerSettings.companyName;
            }

            if (string.IsNullOrEmpty(settings.rootNamespace))
            {
                settings.rootNamespace = EditorSettings.projectGenerationRootNamespace;
            }
        }

        private LineEndingsMode Convert(LineEndings lineEndings)
        {
            switch (lineEndings)
            {
                case LineEndings.Unix:
                    return LineEndingsMode.Unix;

                case LineEndings.Windows:
                    return LineEndingsMode.Windows;

                default:
                    Debug.LogErrorFormat("Found unknown line endings type {0}", lineEndings);
                    return LineEndingsMode.Unix;
            }
        }
    }
}
