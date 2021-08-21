#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LazyGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    #if UNITY
    public class LazyAnimationClip : LazyAsset<AnimationClip>
    #else
    public class LazyAnimationClip : LazyAsset<object>
    #endif
    {
    }
}

#endif
