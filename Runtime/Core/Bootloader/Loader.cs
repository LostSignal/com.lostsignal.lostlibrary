//-----------------------------------------------------------------------
// <copyright file="Loader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Loader : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private List<Loader> dependencies;
        #pragma warning restore 0649
        public List<Loader> Dependencies => this.dependencies;

        public abstract bool IsLoaded { get; }

        public abstract void Load();
    }
}

#endif
