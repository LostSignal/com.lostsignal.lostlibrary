//-----------------------------------------------------------------------
// <copyright file="LazyAsset.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class LazyAsset
    {
#pragma warning disable 0649, CA2235
        [SerializeField] private string assetGuid;
#pragma warning restore 0649, CA2235

        public LazyAsset()
        {
        }

        public LazyAsset(string guid)
        {
            this.assetGuid = guid;
        }

        public object RuntimeKey => this.assetGuid;

        public string AssetGuid
        {
            get
            {
                return this.assetGuid;
            }

            set
            {
#if UNITY
                if (this.assetGuid != null && this.assetGuid != value && Application.isPlaying)
                {
                    Debug.LogError("Changing a LazyAsset's Guid after it has been set will cause issues!");
                }
#endif

                this.assetGuid = value;
            }
        }

        public virtual System.Type Type
        {
            get { return null; }
        }
    }
}
