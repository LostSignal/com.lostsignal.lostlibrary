#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="RuntimeBuildConfig.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.BuildConfig
{
    using System;
    using System.Collections.Generic;
    using Lost.CloudBuild;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public class RuntimeBuildConfig
    {
        public const string FilePath = "Assets/Resources/buildconfig.json";
        public const string ResourcePath = "buildconfig";

        private static RuntimeBuildConfig instance;

        public static RuntimeBuildConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    var configJson = Resources.Load<TextAsset>(ResourcePath);

                    if (configJson != null && string.IsNullOrEmpty(configJson.text) == false)
                    {
                        instance = JsonUtility.FromJson<RuntimeBuildConfig>(configJson.text);
                    }
                }

                return instance;
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        [FormerlySerializedAs("appConfigGuid")]
        [SerializeField] private string buildConfigGuid;

        [FormerlySerializedAs("appConfigName")]
        [SerializeField] private string buildConfigName;

        [SerializeField] private List<KeyValuePair> keyValuePairs;

        private bool isInitialized = false;
        private Dictionary<string, string> values;
        private CloudBuildManifest cloudBuildManifest;
        private string versionAndBuildNumber;
        private string versionAndCommitId;
        private string version;

        public RuntimeBuildConfig()
        {
        }

        public RuntimeBuildConfig(string guid, string name, Dictionary<string, string> variables)
        {
            this.buildConfigGuid = guid;
            this.buildConfigName = name;
            this.cloudBuildManifest = CloudBuildManifest.Find();

            if (variables != null)
            {
                this.keyValuePairs = new List<KeyValuePair>();
                foreach (var keyValuePair in variables)
                {
                    this.keyValuePairs.Add(new KeyValuePair(keyValuePair.Key, keyValuePair.Value));
                }
            }
        }

        public string BuildConfigGuid
        {
            get => this.buildConfigGuid;
            set => this.buildConfigGuid = value;
        }

        public string BuildConfigName
        {
            get => this.buildConfigName;
            set => this.buildConfigName = value;
        }

        public string Version
        {
            get
            {
                this.Initialize();
                return this.version;
            }
        }

        public int BuildNumber
        {
            get
            {
                this.Initialize();
                return this.cloudBuildManifest == null ? 0 : this.cloudBuildManifest.BuildNumber;
            }
        }

        public string CommitId
        {
            get
            {
                this.Initialize();
                return this.cloudBuildManifest?.ScmCommitId;
            }
        }

        public string VersionAndBuildNumber
        {
            get
            {
                this.Initialize();
                return this.versionAndBuildNumber;
            }
        }

        public string VersionAndCommitId
        {
            get
            {
                this.Initialize();
                return this.versionAndCommitId;
            }
        }

        public string GetString(string key)
        {
            this.Initialize();

            if (this.values.TryGetValue(key, out string value))
            {
                return value;
            }

            return null;
        }

        public bool GetBool(string key)
        {
            this.Initialize();
            this.values.TryGetValue(key, out string value);
            return value == "true" || value == "True" || value == "1";
        }

        private void Initialize()
        {
            if (this.isInitialized == false)
            {
                this.isInitialized = true;

                // Caching all the key/value pairs in a dictionary
                this.values = new Dictionary<string, string>();

                for (int i = 0; i < this.keyValuePairs.Count; i++)
                {
                    this.values.Add(this.keyValuePairs[i].Key, this.keyValuePairs[i].Value);
                }

                this.cloudBuildManifest = CloudBuildManifest.Find();
                this.version = Application.version;
                this.versionAndBuildNumber = this.BuildNumber == 0 ? this.Version : string.Format("{0} ({1})", this.Version, this.BuildNumber);
                this.versionAndCommitId = this.CommitId == null ? this.Version : string.Format("{0} ({1})", this.Version, this.CommitId);
            }
        }

        [Serializable]
        public class KeyValuePair
        {
            [SerializeField] private string key;
            [SerializeField] private string value;

            public KeyValuePair() : this(null, null)
            {
            }

            public KeyValuePair(string key, string value)
            {
                this.key = key;
                this.value = value;
            }

            public string Key
            {
                get { return this.key; }
                set { this.key = value; }
            }

            public string Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }
    }
}

#endif
