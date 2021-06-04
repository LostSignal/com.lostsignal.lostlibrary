//-----------------------------------------------------------------------
// <copyright file="SpriteAtlasLoadingManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.U2D;

    public sealed class SpriteAtlasLoadingManager : Manager<SpriteAtlasLoadingManager>
    {
        private Dictionary<string, Action<SpriteAtlas>> unknownAtlasRequests = new Dictionary<string, Action<SpriteAtlas>>();
        private Dictionary<string, Atlas> atlasesMap = null;
        private Settings settings;

        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                yield return ReleasesManager.WaitForInitialization();
                yield return AddressablesManager.WaitForInitialization();

                this.settings = ReleasesManager.Instance.CurrentRelease.SpriteAtlasLoadingManagerSettings;

                this.SetInstance(this);
            }
        }

        public void RegisterAtlas(string tag, string guid)
        {
            Dictionary<string, Atlas> atlasMap = this.GetAtlasMap();
            atlasMap.Add(tag, new Atlas(tag, guid));

            if (this.unknownAtlasRequests.ContainsKey(tag))
            {
                this.RequestAtlas(tag, this.unknownAtlasRequests[tag]);
                this.unknownAtlasRequests.Remove(tag);
            }
        }

        public bool IsAtlasTagLoaded(string tag)
        {
            if (this.GetAtlasMap().TryGetValue(tag, out Atlas atlas))
            {
                return atlas.SpriteAtlas.IsLoaded;
            }

            return false;
        }

        public UnityTask<SpriteAtlas> LoadAtlasTag(string tag)
        {
            if (this.GetAtlasMap().TryGetValue(tag, out Atlas atlas))
            {
                return atlas.SpriteAtlas.Load();
            }

            return null;
        }

        public void UnloadAtlas(string tag)
        {
            if (this.GetAtlasMap().TryGetValue(tag, out Atlas atlas))
            {
                if (atlas.SpriteAtlas.IsLoaded)
                {
                    atlas.SpriteAtlas.Release();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SpriteAtlasManager.atlasRequested += this.RequestAtlas;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SpriteAtlasManager.atlasRequested -= this.RequestAtlas;
        }

        private Dictionary<string, Atlas> GetAtlasMap()
        {
            if (this.atlasesMap == null)
            {
                this.atlasesMap = new Dictionary<string, Atlas>();

                foreach (var atlas in this.settings.Atlases)
                {
                    this.atlasesMap.Add(atlas.Tag, atlas);
                }
            }

            return this.atlasesMap;
        }

        private void RequestAtlas(string tag, Action<SpriteAtlas> callback)
        {
            Dictionary<string, Atlas> atlasMap = this.GetAtlasMap();

            if (atlasMap.TryGetValue(tag, out Atlas atlas))
            {
                if (atlas.SpriteAtlas.IsLoaded)
                {
                    callback?.Invoke(atlas.SpriteAtlas.Load().Value);
                }
                else
                {
                    CoroutineRunner.Instance.StartCoroutine(LoadSpriteAtlasCoroutine());
                }
            }
            else
            {
                if (this.unknownAtlasRequests.ContainsKey(tag) == false)
                {
                    this.unknownAtlasRequests.Add(tag, callback);
                }
            }

            IEnumerator LoadSpriteAtlasCoroutine()
            {
                var loadAtlas = atlas.SpriteAtlas.Load();
                yield return loadAtlas;
                callback?.Invoke(loadAtlas.Value);
            }
        }

        [Serializable]
        public class Atlas
        {
#pragma warning disable 0649
            [SerializeField] private string tag;
            [SerializeField] private LazySpriteAtlas spriteAtlas;
#pragma warning restore 0649

            public Atlas(string tag, string guid)
            {
                this.tag = tag;
                this.spriteAtlas = new LazySpriteAtlas(guid);
            }

            public LazySpriteAtlas SpriteAtlas
            {
                get { return this.spriteAtlas; }
                set { this.spriteAtlas = value; }
            }

            public string Tag
            {
                get { return this.tag; }
                set { this.tag = value; }
            }
        }

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            [SerializeField] private List<Atlas> atlases = new List<Atlas>();
#pragma warning restore 0649

            public List<Atlas> Atlases
            {
                get => this.atlases;
                set => this.atlases = value;
            }
        }
    }
}

#endif
