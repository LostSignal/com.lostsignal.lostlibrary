//-----------------------------------------------------------------------
// <copyright file="AppConfig.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [Serializable]
    public class AppConfig
    {
#pragma warning disable 0649
        [SerializeReference] private List<AppConfigSettings> settings = new List<AppConfigSettings>();
        [SerializeField] private string id = Guid.NewGuid().ToString();
        [SerializeField] private string name;
        [SerializeField] private bool isDefault;
        [SerializeField] private string parentId;
        [SerializeField] private List<string> customDefines = new List<string>();
        [NonSerialized] private bool showInherited;
#pragma warning restore 0649

        public string Id => this.id;

        public string Name
        {
            get => this.name;
            set => this.name = value;
        }

        public string FullName
        {
            get
            {
                if (this.Parent == null)
                {
                    return this.name;
                }
                else
                {
                    return $"{this.Parent.FullName}/{this.name}";
                }
            }
        }

        public int Depth
        {
            get
            {
                int depth = 0;
                var appConfig = this;

                while (appConfig.Parent != null)
                {
                    depth++;
                    appConfig = appConfig.Parent;
                }

                return depth;
            }
        }

        public string ParentId
        {
            get => this.parentId;
            set => this.parentId = value;
        }

        public AppConfig Parent => LostLibrary.AppConfigs.AppConfigs.FirstOrDefault(x => x.Id == this.parentId);

        public bool IsDefault
        {
            get => this.isDefault;
            set => this.isDefault = value;
        }

        public List<string> Defines => this.customDefines;

        public List<AppConfigSettings> Settings => this.settings;

        public string SafeName => this.name?.Replace(" ", string.Empty) ?? string.Empty;

        public bool ShowInherited
        {
            get => this.showInherited;
            set => this.showInherited = value;
        }

        public T GetSettings<T>() where T : AppConfigSettings
        {
            return this.GetSettings(typeof(T)) as T;
        }

        public AppConfigSettings GetSettings(System.Type type)
        {
            bool isInherited;
            return GetSettings(type, out isInherited);
        }

        public AppConfigSettings GetSettings(System.Type type, out bool isInherited)
        {
            if (this.settings != null)
            {
                foreach (var settings in this.settings)
                {
                    if (settings != null && settings.GetType() == type)
                    {
                        isInherited = false;
                        return settings;
                    }
                }
            }

            isInherited = true;
            return RecursiveGetSettings(this.Parent, type);
        }

        private AppConfigSettings RecursiveGetSettings(AppConfig parentBuildConfig, System.Type type)
        {
            if (parentBuildConfig == null)
            {
                return null;
            }

            if (parentBuildConfig.settings != null)
            {
                foreach (var settings in parentBuildConfig.settings)
                {
                    if (settings != null && settings.GetType() == type)
                    {
                        return settings;
                    }
                }
            }

            return RecursiveGetSettings(parentBuildConfig.Parent, type);
        }
    }
}
