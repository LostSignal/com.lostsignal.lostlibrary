#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ProjectDefinesHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;

    #if UNITY_EDITOR
    using System.Linq;
    #endif

    public static class ProjectDefinesHelper
    {
        public static void AddDefineToProject(string defineToAdd)
        {
            #if UNITY_EDITOR
            var validGroups = new List<UnityEditor.BuildTargetGroup>
            {
                UnityEditor.BuildTargetGroup.Standalone,
                UnityEditor.BuildTargetGroup.iOS,
                UnityEditor.BuildTargetGroup.Android,
                UnityEditor.BuildTargetGroup.WebGL,
                UnityEditor.BuildTargetGroup.WSA,
                UnityEditor.BuildTargetGroup.PS4,
                UnityEditor.BuildTargetGroup.XboxOne,
                UnityEditor.BuildTargetGroup.tvOS,
                UnityEditor.BuildTargetGroup.Switch,
                UnityEditor.BuildTargetGroup.Lumin,
            };

            foreach (var buildTargetGroup in validGroups)
            {
                string currentDefinesString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                string newDefinesString = GetNewDefinesString(buildTargetGroup);

                if (currentDefinesString != newDefinesString)
                {
                    UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefinesString);
                }
            }

            string GetNewDefinesString(UnityEditor.BuildTargetGroup buildTargetGroup)
            {
                var currentDefines = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();
                currentDefines.AddIfUnique(defineToAdd);
                return string.Join(";", currentDefines);
            }

            #endif
        }
    }
}

#endif
