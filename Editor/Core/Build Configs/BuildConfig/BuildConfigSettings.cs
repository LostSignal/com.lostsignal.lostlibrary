#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="BuildConfigSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public abstract class BuildConfigSettings
    {
        public abstract string DisplayName { get; }

        public abstract bool IsInline { get; }

        public virtual void GetRuntimeConfigSettings(BuildConfig buildConfig, Dictionary<string, string> runtimeConfigSettings)
        {
        }

        public virtual BuildPlayerOptions ChangeBuildPlayerOptions(BuildConfig buildConfig, BuildPlayerOptions options)
        {
            return options;
        }

        public virtual void DrawSettings(BuildConfigSettings settings, SerializedProperty settingsSerializedProperty, float width)
        {
            SerializedObject serializedObject = settingsSerializedProperty.serializedObject;
            SerializedProperty prop = settingsSerializedProperty;

            bool settingIndexSet = false;
            int settingsIndex = int.MinValue;

            while (prop.NextVisible(true))
            {
                if (settingIndexSet == false)
                {
                    settingIndexSet = true;
                    settingsIndex = this.GetSettingsIndex(prop.propertyPath);
                }

                // NOTE [bgish]: Since settings are in an array, NextVisible will gladly go through all all elements even
                //               though we only care about the current one we're on.  So break once we're no longer on the
                //               the current one.
                if (settingsIndex != this.GetSettingsIndex(prop.propertyPath))
                {
                    break;
                }

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
        }

        private int GetSettingsIndex(string propertyPath)
        {
            int startIndex = propertyPath.IndexOf(".settings.Array.data[");

            if (startIndex == -1)
            {
                return -1;
            }

            int endIndex = propertyPath.IndexOf("]", startIndex);

            string arrayIndexString = propertyPath.Substring(startIndex + 21, endIndex - (startIndex + 21));

            return int.TryParse(arrayIndexString, out int result) ? result : -1;
        }
    }
}
