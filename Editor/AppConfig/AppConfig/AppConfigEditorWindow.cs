//-----------------------------------------------------------------------
// <copyright file="AppConfigEditorWindow.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Lost.AppConfig;
    using Lost.EditorGrid;
    using UnityEditor;
    using UnityEngine;

    public class AppConfigEditorWindow : EditorWindow
    {
        private static List<Type> BuildSettingsTypes;
        private static AppConfig.AppConfig SelectedConfig;

        static AppConfigEditorWindow()
        {
            BuildSettingsTypes = new List<Type>();

            foreach (var t in TypeUtil.GetAllTypesOf<AppConfigSettings>())
            {
                BuildSettingsTypes.Add(t);
            }

            BuildSettingsTypes = BuildSettingsTypes.OrderBy((t) =>
            {
                var attribute = t.GetCustomAttributes(typeof(AppConfigSettingsOrderAttribute), true).FirstOrDefault() as AppConfigSettingsOrderAttribute;
                return attribute == null ? 1000 : attribute.Order;
            }).ToList();
        }

        [MenuItem("Tools/Lost/Tools/App Configs Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<AppConfigEditorWindow>(false, "App Configs");
        }

        private void OnGUI()
        {
            var appConfigs = LostLibrary.AppConfigs.AppConfigs;

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

                if (appConfigs.IsNullOrEmpty() == false)
                {
                    foreach (var appConfig in appConfigs.OrderBy(x => x.FullName))
                    {
                        GUI.backgroundColor = (appConfig == SelectedConfig) ? color_selected : Color.clear;

                        StringBuilder depthString = new StringBuilder();
                        for (int i = 0; i < appConfig.Depth * 4; i++)
                        {
                            depthString.Append(" ");
                        }

                        if (GUILayout.Button(depthString + appConfig.Name, itemStyle))
                        {
                            SelectedConfig = appConfig;
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
                SerializedObject editorAppConfigsSO = new SerializedObject(LostLibrary.AppConfigs);
                SerializedProperty appConfigsSerializedProperty = editorAppConfigsSO.FindProperty("appConfigs");

                SerializedProperty selectSerializedProperty = null;

                if (SelectedConfig != null)
                {
                    for (int i = 0; i < appConfigsSerializedProperty.arraySize; i++)
                    {
                        var element = appConfigsSerializedProperty.GetArrayElementAtIndex(i);

                        if (element.FindPropertyRelative("id").stringValue == SelectedConfig.Id)
                        {
                            selectSerializedProperty = element;
                            break;
                        }
                    }

                    using (new LabelWidthScope(220))
                    {
                        this.DrawAppConfig(SelectedConfig, selectSerializedProperty, rightSideWidth);
                    }
                }
            }
        }

        private void DrawAppConfig(AppConfig.AppConfig appConfig, SerializedProperty appConfigSerialiedProperty, float currentViewWidth)
        {
            appConfig.ShowInherited = EditorGUILayout.Toggle("Show Inherited", appConfig.ShowInherited);
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.TextField("Id", appConfig.Id);
            }

            appConfig.Name = EditorGUILayout.TextField("Name", appConfig.Name);
            appConfig.ParentId = EditorGUILayout.TextField("Parent Id", appConfig.ParentId);
            EditorGUILayout.PropertyField(appConfigSerialiedProperty.FindPropertyRelative("customDefines"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            this.DrawAppConfigSettings(appConfig, appConfigSerialiedProperty.FindPropertyRelative("settings"), currentViewWidth);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(LostLibrary.AppConfigs);
            }
        }

        private void DrawAppConfigSettings(AppConfig.AppConfig appConfig, SerializedProperty settingsSerializedProperty, float currentViewWidth)
        {
            List<Type> settingsToAdd = new List<Type>();

            foreach (var settingsType in BuildSettingsTypes)
            {
                AppConfigSettings settings = appConfig.GetSettings(settingsType, out bool isInherited);

                if (settings == null)
                {
                    settingsToAdd.Add(settingsType);
                }
                else
                {
                    // Checking if we should skip this
                    if (isInherited && appConfig.ShowInherited == false)
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

                    this.DrawAppConfigSettings(appConfig, settings, currentSettings, isInherited, currentViewWidth, out bool didDeleteSettings);

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
            this.DrawSettingsButtons(appConfig, currentViewWidth, settingsToAdd);
        }

        private void DrawAppConfigSettings(AppConfig.AppConfig appConfig, AppConfigSettings settings, SerializedProperty settingsSerializedProperty, bool isInherited, float currentViewWidth, out bool didDeleteSettings)
        {
            didDeleteSettings = false;

            bool foldoutVisible = true;
            Rect boxRect = new Rect();

            var verticleHelper = settings.IsInline ?
                (IDisposable)new EditorGUILayout.HorizontalScope("box") :
                (IDisposable)new FoldoutScope(settings.DisplayName.GetHashCode(), settings.DisplayName, out foldoutVisible, out boxRect, false);

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
                        appConfig.Settings.Add(Activator.CreateInstance(settings.GetType()) as AppConfigSettings);
                    }
                }
                else
                {
                    if (ButtonUtil.DrawDeleteButton(buttonRect))
                    {
                        appConfig.Settings.Remove(settings);
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

        private void DrawSettingsButtons(AppConfig.AppConfig appConfig, float currentViewWidth, List<Type> settingsToAdd)
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
                            appConfig.Settings.Add(Activator.CreateInstance(settingsType) as AppConfigSettings);
                        }
                    }
                }
            }
        }
    }
}
