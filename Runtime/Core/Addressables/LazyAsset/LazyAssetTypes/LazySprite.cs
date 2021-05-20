//-----------------------------------------------------------------------
// <copyright file="LazySprite.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    #if UNITY_2018_3_OR_NEWER
    public class LazySprite : LazyAsset<Sprite>
    #else
    public class LazySprite : LazyAsset<object>
    #endif
    {
        #if UNITY_2018_3_OR_NEWER
        public LazySprite()
        {
        }

        public LazySprite(string guid) : base(guid)
        {
        }

        #endif
    }
}
