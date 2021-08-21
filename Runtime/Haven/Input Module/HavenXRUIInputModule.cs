#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="HavenXRUIInputModule.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Haven
{
    [UnityEngine.AddComponentMenu("")]
#if USING_UNITY_XR_INTERACTION_TOOLKIT
    public class HavenXRUIInputModule : UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule
#else
    public class HavenXRUIInputModule : UnityEngine.MonoBehaviour
#endif
    {
    }
}

#endif
