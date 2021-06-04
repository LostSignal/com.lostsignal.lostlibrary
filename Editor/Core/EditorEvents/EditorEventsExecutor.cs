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
    using UnityEngine.SceneManagement;

    #if UNITY_ANDROID
    using UnityEditor.Android;
    #endif

    [InitializeOnLoad]
    public class EditorEventsExecutor :
        #if UNITY_ANDROID
        IPostGenerateGradleAndroidProject,
        #endif
        IPreprocessBuildWithReport,
        IPostprocessBuildWithReport,
        IProcessSceneWithReport
    {
        public int callbackOrder => 10;

        int IOrderedCallback.callbackOrder => throw new NotImplementedException();

        static EditorEventsExecutor()
	    {
		    EditorApplication.delayCall += () => ExecuteAttribute<EditorEvents.OnDomainReloadAttribute>();
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
	    }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            ExecuteAttribute<EditorEvents.OnPreprocessBuildAttribute>(report);
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            ExecuteAttribute<EditorEvents.OnPostprocessBuildAttribute>(report);
        }

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            ExecuteAttribute<EditorEvents.OnProcessSceneAttribute>(scene, report);
        }
        
        #if UNITY_ANDROID
        void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string gradlePath)
        {
            ExecuteAttribute<EditorEvents.OnPostGenerateGradleAndroidProjectAttribute>(gradlePath);
        }
        #endif

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

        private static void ExecuteAttribute<T>(params object[] parameters) where T : System.Attribute
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
                                // TODO [bgish]: Pass in parameters if they match
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
    }
}
