//-----------------------------------------------------------------------
// <copyright file="Releases.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class Releases : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private List<Release> releases = new List<Release> { new Release { AppVersion = "0.1.0" } };
#pragma warning restore 0649

        public Release CurrentRelease => this.releases.LastOrDefault();
    }
}
