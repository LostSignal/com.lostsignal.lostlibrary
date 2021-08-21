#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="EditorEventsExecutor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Linq;
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
        int IOrderedCallback.callbackOrder => 10;

        static EditorEventsExecutor()
        {
            EditorApplication.delayCall += () => ExecuteAttribute<EditorEvents.OnDomainReloadAttribute>();
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        public static void ExecuteAttribute<T>(params object[] parameters) where T : System.Attribute
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

                ExecuteAssembly(assembly);
            }

            void ExecuteAssembly(Assembly assembly)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetRuntimeMethods().Where(x => x.IsStatic && x.GetCustomAttribute<T>() != null))
                    {
                        try
                        {
                            ExecuteMethod(method);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Exception Executing Edtior Event {typeof(T).Name}");
                            Debug.LogException(ex);
                        }
                    }
                }
            }

            void ExecuteMethod(MethodInfo method)
            {
                var methodParameters = method.GetParameters();

                // Special Case for Android Gradle builds
                if (typeof(T) == typeof(EditorEvents.OnPostGenerateGradleAndroidProjectAttribute) &&
                    methodParameters?.Length == 1 &&
                    methodParameters[0].ParameterType == typeof(string))
                {
                    method.Invoke(null, parameters);
                    return;
                }

                // Special Case for Pre/Post Process Build
                if ((typeof(T) == typeof(EditorEvents.OnPreprocessBuildAttribute) ||
                     typeof(T) == typeof(EditorEvents.OnPostprocessBuildAttribute)) &&
                    methodParameters?.Length == 1 &&
                    methodParameters[0].ParameterType == typeof(BuildReport))
                {
                    method.Invoke(null, parameters);
                    return;
                }

                // Special Case for Process Scene
                if (typeof(T) == typeof(EditorEvents.OnProcessSceneAttribute))
                {
                    var scene = parameters[0];
                    var buildReport = parameters[1];

                    if (methodParameters?.Length == 1 &&
                        methodParameters[0].ParameterType == typeof(Scene))
                    {
                        // Scene
                        method.Invoke(null, new object[] { scene });
                        return;
                    }
                    else if (methodParameters?.Length == 1 &&
                             methodParameters[0].ParameterType == typeof(BuildReport))
                    {
                        // BuildReport
                        method.Invoke(null, new object[] { buildReport });
                        return;
                    }
                    else if (methodParameters?.Length == 2 &&
                             methodParameters[0].ParameterType == typeof(BuildReport) &&
                             methodParameters[1].ParameterType == typeof(Scene))
                    {
                        // BuildReport, Scene
                        method.Invoke(null, new object[] { buildReport, scene });
                        return;
                    }
                    else if (methodParameters?.Length == 2 &&
                             methodParameters[0].ParameterType == typeof(Scene) &&
                             methodParameters[1].ParameterType == typeof(BuildReport))
                    {
                        // Scene, BuildReport
                        method.Invoke(null, new object[] { scene, buildReport });
                        return;
                    }
                }

                method.Invoke(null, null);
            }
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
    }
}
