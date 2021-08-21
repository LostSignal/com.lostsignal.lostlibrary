#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="BuildConfig.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [Serializable]
    public class BuildConfig
    {
#pragma warning disable 0649
        [SerializeReference] private List<BuildConfigSettings> settings = new List<BuildConfigSettings>();
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
                var buildConfig = this;

                while (buildConfig.Parent != null)
                {
                    depth++;
                    buildConfig = buildConfig.Parent;
                }

                return depth;
            }
        }

        public string ParentId
        {
            get => this.parentId;
            set => this.parentId = value;
        }

        public BuildConfig Parent => LostLibrary.BuildConfigs.BuildConfigs.FirstOrDefault(x => x.Id == this.parentId);

        public bool IsDefault
        {
            get => this.isDefault;
            set => this.isDefault = value;
        }

        public List<string> Defines => this.customDefines;

        public List<BuildConfigSettings> Settings => this.settings;

        public string SafeName => this.name?.Replace(" ", string.Empty) ?? string.Empty;

        public bool ShowInherited
        {
            get => this.showInherited;
            set => this.showInherited = value;
        }

        public T GetSettings<T>() where T : BuildConfigSettings
        {
            return this.GetSettings(typeof(T)) as T;
        }

        public BuildConfigSettings GetSettings(System.Type type)
        {
            bool isInherited;
            return GetSettings(type, out isInherited);
        }

        public BuildConfigSettings GetSettings(System.Type type, out bool isInherited)
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

        private BuildConfigSettings RecursiveGetSettings(BuildConfig parentBuildConfig, System.Type type)
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
