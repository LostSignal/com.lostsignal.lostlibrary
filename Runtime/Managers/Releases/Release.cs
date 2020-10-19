//-----------------------------------------------------------------------
// <copyright file="Release.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using Lost.PlayFab;
    using Newtonsoft.Json;
    using UnityEngine;

    public enum TitleDataSerializationMethod
    {
        PlayFab,
        JsonDotNet,
        Unity,
    }

    [Serializable]
    public class Release
    {
#pragma warning disable 0649
        [SerializeField] private string appVersion;
        [SerializeField] private PlayFabData playFab = new PlayFabData();
        [SerializeField] private string ablyClientKey;
#pragma warning restore 0649

        public string dataVersion;
        public bool forceAppUpdate;
        public string addressablesUrl;
        public SerializableDictionary<string, string> customData;
        public List<LazyAsset> startupAssets;
        public List<LazySpriteAtlas> startupSpriteAtlases;

        public string AppVersion
        {
            get => this.appVersion;
            set => this.appVersion = value;
        }

        public PlayFabData PlayFab
        {
            get => this.playFab;
            set => this.playFab = value;
        }

        public string AblyClientKey
        {
            get => this.ablyClientKey;
            set => this.ablyClientKey = value;
        }

        [Serializable]
        public class PlayFabData
        {
#pragma warning disable 0649
            [SerializeField] private string catalogVersion;
            [SerializeField] private List<TitleDataKeys> titleDataKeys;
            [SerializeField] private List<MatchmakingConfig> matchmakingConfigs;

            [SerializeField]
            private SerializableDictionary<string, LoginURLs> loginURLs = new SerializableDictionary<string, LoginURLs>
            {
                { "Dev", new Release.LoginURLs() },
                { "Live", new Release.LoginURLs() },
            };
#pragma warning restore 0649

            [JsonIgnore]
            public string TitleId
            {
                get => AppConfig.RuntimeAppConfig.Instance.GetTitleId();
            }

            public string CatalogVersion
            {
                get => this.catalogVersion;
                set => this.catalogVersion = value;
            }

            public List<TitleDataKeys> TitleDataKeys
            {
                get => this.titleDataKeys;
                set => this.titleDataKeys = value;
            }

            public List<MatchmakingConfig> MatchmakingConfigs
            {
                get => this.matchmakingConfigs;
                set => this.matchmakingConfigs = value;
            }

            public SerializableDictionary<string, LoginURLs> LoginURLs
            {
                get => this.loginURLs;
                set => this.loginURLs = value;
            }


#if UNITY_EDITOR
            public List<ITitleData> TitleDatas;
#endif
        }

        [Serializable]
        public class LoginURLs
        {
            public string RequestLoginCodeUrl;
            public string LoginWithCodeUrl;
        }

        public interface ITitleData
        {
            string TitleDataKey { get; }

            bool IsCompressed { get; }

            bool LoadAtStartup { get; }

            TitleDataSerializationMethod SerializationMethod { get; }
        }

        [Serializable]
        public class TitleDataKeys
        {
            public string TitleDataKey;
            public bool IsCompressed;
            public bool LoadAtStartup;
            public TitleDataSerializationMethod SerializationMethod;
        }

        [Serializable]
        public class MatchmakingConfig
        {
            public string ConfigName { get; set; }

            public string GameMode { get; set; }

            public string BuildVersion { get; set; }

            public List<int> Regions { get; set; }

            public bool StartNewIfNoneFound { get; set; }
        }
    }
}
