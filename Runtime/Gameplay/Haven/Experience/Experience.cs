//-----------------------------------------------------------------------
// <copyright file="Experience.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace HavenXR
{
    using System;
    using System.Collections.Generic;
    using Lost;
    using PlayFab.ClientModels;
    using UnityEngine;

    [Serializable]
    public class Experience
    {
        #pragma warning disable 0649
        private string name;
        private string description;
        //// private LazySprite icon;
        //// private LazySprite[] images;
        private LazyScene[] scenes;
        private LazyScene[] pcScenes;
        private LazyScene[] arMobileScenes;
        #pragma warning restore 0649
    }
}
