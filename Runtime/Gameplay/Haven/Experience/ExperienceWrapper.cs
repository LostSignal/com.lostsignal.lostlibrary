//-----------------------------------------------------------------------
// <copyright file="ExperienceWrapper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace HavenXR
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Experience", menuName = "Haven/Haven Experience")]
    public class ExperienceWrapper : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private Experience experience;
#pragma warning restore 0649

        public Experience Experience => this.experience;
    }
}

#endif
