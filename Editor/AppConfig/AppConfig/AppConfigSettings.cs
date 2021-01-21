//-----------------------------------------------------------------------
// <copyright file="BuildConfigSettings.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public abstract class AppConfigSettings
    {
        public abstract string DisplayName { get; }

        public abstract bool IsInline { get; }

        public virtual void GetRuntimeConfigSettings(AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
        }

        public virtual void OnPreproccessBuild(AppConfig appConfig, BuildReport buildReport)
        {
        }

        public virtual void OnPostprocessBuild(AppConfig appConfig, BuildReport buildReport)
        {
        }

        public virtual void OnProcessScene(AppConfig appConfig, Scene scene, BuildReport report)
        {
        }

        public virtual void OnPostGenerateGradleAndroidProject(AppConfig appConfig, string gradlePath)
        {
        }

        public virtual void InitializeOnLoad(AppConfig appConfig)
        {
        }

        public virtual void OnUserBuildInitiated(AppConfig appConfig)
        {
        }

        public virtual void OnUnityCloudBuildInitiated(AppConfig appConfig)
        {
        }

        public virtual void OnEnteringPlayMode(AppConfig appConfig)
        {
        }

        public virtual void OnExitingPlayMode(AppConfig appConfig)
        {
        }

        public virtual BuildPlayerOptions ChangeBuildPlayerOptions(AppConfig appConfig, BuildPlayerOptions options)
        {
            return options;
        }

        public virtual void DrawSettings(AppConfigSettings settings, SerializedProperty settingsSerializedProperty, float width)
        {
            SerializedObject serializedObject = settingsSerializedProperty.serializedObject;
            SerializedProperty prop = settingsSerializedProperty;

            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script" || prop.hasVisibleChildren)
                {
                    continue;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(prop, false, GUILayout.Width(width));
                }
            }

            serializedObject.ApplyModifiedProperties();

            // SerializedObject serializedObject = new SerializedObject(settings);
            // SerializedProperty prop = serializedObject.GetIterator();
            // 
            // while (prop.NextVisible(true))
            // {
            //     if (prop.name == "m_Script" || prop.hasVisibleChildren)
            //     {
            //         continue;
            //     }
            // 
            //     using (new EditorGUILayout.HorizontalScope())
            //     {
            //         EditorGUILayout.PropertyField(prop, false, GUILayout.Width(width));
            //     }
            // }
            // 
            // serializedObject.ApplyModifiedProperties();
        }
    }
}
