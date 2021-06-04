//-----------------------------------------------------------------------
// <copyright file="EditorEventsExecutor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    [InitializeOnLoad]
    public class EditorEventsExecutor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 10;

        static EditorEventsExecutor()
	    {
		    EditorApplication.delayCall += () => ExecuteAttribute<EditorEvents.OnUnityLoadedAttribute>();
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
	    }

        private static void PlayModeStateChanged(PlayModeStateChange state)
        {
	        if (EditorApplication.isPlaying == false && EditorApplication.isPlayingOrWillChangePlaymode)
	        {
                ExecuteAttribute<EditorEvents.OnEnterPlayModeAttribute>();
	        }
	        else if (state == PlayModeStateChange.ExitingPlayMode)
	        {
                ExecuteAttribute<EditorEvents.OnExitingPlayModeAttribute>();
                EditorApplication.delayCall += WaitForPlayModeExit;
	        }
        }

        private static void ExecuteAttribute<T>() where T : System.Attribute
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = assembly.FullName.Substring(0, assembly.FullName.IndexOf(","));

                if (name == "System" ||
                    name == "mscorlib" ||
                    name == "UnityEngine" ||
                    name == "UnityEditor" ||
                    name.StartsWith("Mono.") ||
                    name.StartsWith("Unity.") ||
                    name.StartsWith("System.") ||
                    name.StartsWith("UnityEditor.") ||
                    name.StartsWith("UnityEngine."))
                {
                    continue;
                }

                Execute(assembly);
            }

            void Execute(Assembly assembly)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetRuntimeMethods())
                    {
                        if (method.IsStatic && method.GetCustomAttribute<T>() != null)
                        {
                            try
                            {
                                method.Invoke(null, null);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Exception Executing Edtior Event {typeof(T).Name}");
                                Debug.LogException(ex);
                            }                            
                        }
                    }
                }
            }
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            ExecuteAttribute<EditorEvents.OnPreprocessBuildAttribute>();
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            ExecuteAttribute<EditorEvents.OnPostprocessBuildAttribute>();
        }

        private static void WaitForPlayModeExit()
        {
            if (Application.isPlaying)
            {
                EditorApplication.delayCall += WaitForPlayModeExit;
            }
            else
            {
                ExecuteAttribute<EditorEvents.OnExitPlayModeAttribute>();
            }
        }
    }
}
