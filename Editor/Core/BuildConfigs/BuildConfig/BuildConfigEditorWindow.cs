//-----------------------------------------------------------------------
// <copyright file="BuildConfigEditorWindow.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Lost.BuildConfig;
    using Lost.EditorGrid;
    using UnityEditor;
    using UnityEngine;

    public class BuildConfigEditorWindow : EditorWindow
    {
        private static List<Type> BuildSettingsTypes;
        private static BuildConfig.BuildConfig SelectedConfig;

        static BuildConfigEditorWindow()
        {
            BuildSettingsTypes = new List<Type>();

            foreach (var t in TypeUtil.GetAllTypesOf<BuildConfigSettings>())
            {
                BuildSettingsTypes.Add(t);
            }

            BuildSettingsTypes = BuildSettingsTypes.OrderBy((t) =>
            {
                var attribute = t.GetCustomAttributes(typeof(BuildConfigSettingsOrderAttribute), true).FirstOrDefault() as BuildConfigSettingsOrderAttribute;
                return attribute == null ? 1000 : attribute.Order;
            }).ToList();
        }

        [MenuItem("Tools/Lost/Tools/Build Configs Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<BuildConfigEditorWindow>(false, "Build Configs");
        }

        private void OnGUI()
        {
            var buildConfigs = LostLibrary.BuildConfigs.BuildConfigs;

            float columnWidth = 150;
            float padding = 3;

            float leftSideX = padding;
            float leftSideY = padding;
            float leftSideWidth = columnWidth - (2 * padding);
            float leftSideHeight = this.position.height - (2 * padding);

            using (new GUILayout.AreaScope(new Rect(leftSideX, leftSideY, leftSideWidth, leftSideHeight)))
            {
                // https://forum.unity.com/threads/how-to-make-own-list-ui-in-editor-window.461428/
                Color color_default = GUI.backgroundColor;
                Color color_selected = Color.gray;
                GUIStyle itemStyle = new GUIStyle(GUI.skin.button);
                itemStyle.alignment = TextAnchor.MiddleLeft;
                itemStyle.active.background = itemStyle.normal.background;
                itemStyle.margin = new RectOffset(0, 0, 0, 0);

                if (buildConfigs.IsNullOrEmpty() == false)
                {
                    foreach (var buildConfig in buildConfigs.OrderBy(x => x.FullName))
                    {
                        GUI.backgroundColor = (buildConfig == SelectedConfig) ? color_selected : Color.clear;

                        StringBuilder depthString = new StringBuilder();
                        for (int i = 0; i < buildConfig.Depth * 4; i++)
                        {
                            depthString.Append(" ");
                        }

                        if (GUILayout.Button(depthString + buildConfig.Name, itemStyle))
                        {
                            SelectedConfig = buildConfig;
                        }

                        GUI.backgroundColor = color_default; //this is to avoid affecting other GUIs outside of the list
                    }
                }
            }

            float rightSideX = columnWidth + padding;
            float rightSideY = padding;
            float rightSideWidth = this.position.width - columnWidth - (2.0f * padding);
            float rightSideHeight = this.position.height - (2.0f * padding);

            using (new GUILayout.AreaScope(new Rect(rightSideX, rightSideY, rightSideWidth, rightSideHeight)))
            {
                SerializedObject editorBuildConfigsSO = new SerializedObject(LostLibrary.BuildConfigs);
                SerializedProperty buildConfigsSerializedProperty = editorBuildConfigsSO.FindProperty("buildConfigs");

                SerializedProperty selectSerializedProperty = null;

                if (SelectedConfig != null)
                {
                    for (int i = 0; i < buildConfigsSerializedProperty.arraySize; i++)
                    {
                        var element = buildConfigsSerializedProperty.GetArrayElementAtIndex(i);

                        if (element.FindPropertyRelative("id").stringValue == SelectedConfig.Id)
                        {
                            selectSerializedProperty = element;
                            break;
                        }
                    }

                    using (new LabelWidthScope(220))
                    {
                        this.DrawBuildConfig(SelectedConfig, selectSerializedProperty, rightSideWidth);
                    }
                }
            }
        }

        private void DrawBuildConfig(BuildConfig.BuildConfig buildConfig, SerializedProperty buildConfigSerialiedProperty, float currentViewWidth)
        {
            buildConfig.ShowInherited = EditorGUILayout.Toggle("Show Inherited", buildConfig.ShowInherited);
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.TextField("Id", buildConfig.Id);
            }

            buildConfig.Name = EditorGUILayout.TextField("Name", buildConfig.Name);
            buildConfig.ParentId = EditorGUILayout.TextField("Parent Id", buildConfig.ParentId);
            EditorGUILayout.PropertyField(buildConfigSerialiedProperty.FindPropertyRelative("customDefines"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            this.DrawBuildConfigSettings(buildConfig, buildConfigSerialiedProperty.FindPropertyRelative("settings"), currentViewWidth);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(LostLibrary.BuildConfigs);
            }
        }

        private void DrawBuildConfigSettings(BuildConfig.BuildConfig buildConfig, SerializedProperty settingsSerializedProperty, float currentViewWidth)
        {
            List<Type> settingsToAdd = new List<Type>();

            foreach (var settingsType in BuildSettingsTypes)
            {
                BuildConfigSettings settings = buildConfig.GetSettings(settingsType, out bool isInherited);

                if (settings == null)
                {
                    settingsToAdd.Add(settingsType);
                }
                else
                {
                    // Checking if we should skip this
                    if (isInherited && buildConfig.ShowInherited == false)
                    {
                        continue;
                    }

                    SerializedProperty currentSettings = null;

                    for (int i = 0; i < settingsSerializedProperty.arraySize; i++)
                    {
                        var s = settingsSerializedProperty.GetArrayElementAtIndex(i);

                        if (s.managedReferenceFullTypename.EndsWith(settingsType.FullName))
                        {
                            currentSettings = s;
                        }
                    }

                    this.DrawBuildConfigSettings(buildConfig, settings, currentSettings, isInherited, currentViewWidth, out bool didDeleteSettings);

                    if (didDeleteSettings)
                    {
                        break;
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Settings");
            this.DrawSettingsButtons(buildConfig, currentViewWidth, settingsToAdd);
        }

        private void DrawBuildConfigSettings(BuildConfig.BuildConfig buildConfig, BuildConfigSettings settings, SerializedProperty settingsSerializedProperty, bool isInherited, float currentViewWidth, out bool didDeleteSettings)
        {
            didDeleteSettings = false;

            bool foldoutVisible = true;
            Rect boxRect = new Rect();

            var verticleHelper = settings.IsInline ?
                (IDisposable) new EditorGUILayout.HorizontalScope("box") :
                (IDisposable) new FoldoutScope(settings.DisplayName.GetHashCode(), settings.DisplayName, out foldoutVisible, out boxRect, false);

            using (verticleHelper)
            {
                float y = settings.IsInline ? (verticleHelper as EditorGUILayout.HorizontalScope).rect.y : boxRect.y;

                // Drawing the button
                float buttonSize = 14;
                float rightPadding = 25;
                float topPadding = 2;

                Rect buttonRect = new Rect(new Vector2(currentViewWidth - rightPadding, y + topPadding), new Vector2(buttonSize, buttonSize));

                if (isInherited)
                {
                    if (ButtonUtil.DrawAddButton(buttonRect))
                    {
                        buildConfig.Settings.Add(Activator.CreateInstance(settings.GetType()) as BuildConfigSettings);
                    }
                }
                else
                {
                    if (ButtonUtil.DrawDeleteButton(buttonRect))
                    {
                        buildConfig.Settings.Remove(settings);
                        didDeleteSettings = true;
                        return;
                    }
                }

                // Breaking out if we're not suppose to show the foldout
                if (foldoutVisible == false)
                {
                    return;
                }

                // Iterating and displaying all properties
                using (new EditorGUI.DisabledGroupScope(isInherited))
                {
                    float width = currentViewWidth - (settings.IsInline ? 60 : 15);
                    settings.DrawSettings(settings, settingsSerializedProperty, width);
                }
            }
        }

        private void DrawSettingsButtons(BuildConfig.BuildConfig buildConfig, float currentViewWidth, List<Type> settingsToAdd)
        {
            int buttonWidth = (int)(currentViewWidth / 3.0f);

            for (int i = 0; i < settingsToAdd.Count; i += 3)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int index = i + j;

                        if (index >= settingsToAdd.Count)
                        {
                            break;
                        }

                        var settingsType = settingsToAdd[index];

                        string buttonName = settingsType.Name;

                        if (buttonName.EndsWith("Settings"))
                        {
                            buttonName = buttonName.Replace("Settings", string.Empty);
                        }

                        if (GUILayout.Button(buttonName, GUILayout.Width(buttonWidth)))
                        {
                            buildConfig.Settings.Add(Activator.CreateInstance(settingsType) as BuildConfigSettings);
                        }
                    }
                }
            }
        }
    }
}
