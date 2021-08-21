#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="EditorBuildConfigDefinesHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;

    public static class EditorBuildConfigDefinesHelper
    {
        public static void UpdateProjectDefines()
        {
            if (EditorBuildConfigs.ActiveBuildConfig == null || LostLibrary.BuildConfigs.BuildConfigs == null)
            {
                return;
            }

            HashSet<string> activeDefines = new HashSet<string>();
            HashSet<string> definesToRemove = new HashSet<string>();

            GetActiveDefines(EditorBuildConfigs.ActiveBuildConfig, activeDefines);
            GetAllDefines(LostLibrary.BuildConfigs.BuildConfigs, definesToRemove);

            foreach (var define in activeDefines)
            {
                definesToRemove.Remove(define);
            }

            UpdateProjectDefines(activeDefines, definesToRemove);
        }

        public static void UpdateProjectDefines(HashSet<string> definesToAdd, HashSet<string> definesToRemove)
        {
            foreach (var buildTargetGroup in BuildTargetGroupUtil.GetValid())
            {
                string currentDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                string definesString = GetDefinesString(buildTargetGroup, definesToAdd, definesToRemove);

                if (currentDefinesString != definesString)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, definesString);
                }
            }
        }

        private static void GetActiveDefines(BuildConfig buildConfig, HashSet<string> defines)
        {
            if (buildConfig != null)
            {
                if (buildConfig.Defines != null)
                {
                    foreach (var define in buildConfig.Defines)
                    {
                        defines.Add(define);
                    }
                }

                GetActiveDefines(buildConfig.Parent, defines);
            }
        }

        private static void GetAllDefines(List<BuildConfig> buildConfigs, HashSet<string> defines)
        {
            foreach (var buildConfig in buildConfigs)
            {
                if (buildConfig != null && buildConfig.Defines != null)
                {
                    foreach (var define in buildConfig.Defines)
                    {
                        defines.Add(define);
                    }
                }
            }
        }

        private static string GetDefinesString(BuildTargetGroup buildTargetGroup, HashSet<string> definesToAdd, HashSet<string> definesToRemove)
        {
            var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();

            foreach (var define in definesToAdd)
            {
                if (currentDefines.Contains(define) == false)
                {
                    currentDefines.Add(define);
                }
            }

            foreach (var define in definesToRemove)
            {
                if (currentDefines.Contains(define))
                {
                    currentDefines.Remove(define);
                }
            }

            currentDefines.Sort();

            return string.Join(";", currentDefines);
        }
    }
}
