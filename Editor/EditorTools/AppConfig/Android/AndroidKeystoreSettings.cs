//-----------------------------------------------------------------------
// <copyright file="AndroidKeystoreSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(310)]
    public class AndroidKeystoreSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        #if UNITY_2019_1_OR_NEWER
        [SerializeField] private bool useCustomKeystore = true;
        #endif

        [Header("KeyStore")]
        [SerializeField] private string keystoreFile;  // relative path
        [SerializeField] private string keystorePassword;

        [Header("KeyAlias")]
        [SerializeField] private string keyAliasName;
        [SerializeField] private string keyAliasePassword;
        #pragma warning restore 0649

        public override string DisplayName => "Android Keystore";
        public override bool IsInline => false;

        public override void InitializeOnLoad(AppConfig.AppConfig appConfig)
        {
            var settings = appConfig.GetSettings<AndroidKeystoreSettings>();

            if (settings != null)
            {
                #if UNITY_2019_1_OR_NEWER
                PlayerSettings.Android.useCustomKeystore = settings.useCustomKeystore;
                #endif

                PlayerSettings.Android.keystoreName = settings.keystoreFile;
                PlayerSettings.Android.keystorePass = settings.keystorePassword;
                PlayerSettings.Android.keyaliasName = settings.keyAliasName;
                PlayerSettings.Android.keyaliasPass = settings.keyAliasePassword;
            }
        }
    }
}
