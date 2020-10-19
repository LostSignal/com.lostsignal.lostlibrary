//-----------------------------------------------------------------------
// <copyright file="FoldoutHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.EditorGrid
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class FoldoutScope : IDisposable
    {
        private static Dictionary<int, bool> foldouts = new Dictionary<int, bool>();
        private static GUIStyle titleGuiStyle = null;

        public FoldoutScope(int foldoutId, string title, float width, out bool visible, bool defaultVisible = false)
        {
            // making sure this foldoutId is in the list
            if (foldouts.ContainsKey(foldoutId) == false)
            {
                foldouts.Add(foldoutId, defaultVisible);
            }

            Rect position = EditorGUILayout.BeginVertical("box", GUILayout.Width(width));

            this.DrawTitle(position, foldoutId, title);

            visible = foldouts[foldoutId];
        }

        public FoldoutScope(int foldoutId, string title, out bool visible, bool defaultVisible = false)
        {
            // making sure this foldoutId is in the list
            if (foldouts.ContainsKey(foldoutId) == false)
            {
                foldouts.Add(foldoutId, defaultVisible);
            }

            Rect position = EditorGUILayout.BeginVertical("box");

            this.DrawTitle(position, foldoutId, title);

            visible = foldouts[foldoutId];
        }

        public FoldoutScope(int foldoutId, string title, out bool visible, out Rect position, bool defaultVisible = false)
        {
            // making sure this foldoutId is in the list
            if (foldouts.ContainsKey(foldoutId) == false)
            {
                foldouts.Add(foldoutId, defaultVisible);
            }

            position = EditorGUILayout.BeginVertical("box");

            this.DrawTitle(position, foldoutId, title);

            visible = foldouts[foldoutId];
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }

        private void DrawTitle(Rect position, int foldoutId, string title)
        {
            if (titleGuiStyle == null)
            {
                titleGuiStyle = new GUIStyle(GUI.skin.label);
                titleGuiStyle.alignment = TextAnchor.LowerCenter;
                titleGuiStyle.stretchWidth = true;
                titleGuiStyle.border = new RectOffset();

                if (EditorColorUtil.IsProTheme())
                {
                    titleGuiStyle.normal.textColor = Color.white;
                }
                else
                {
                    titleGuiStyle.normal.textColor = Color.black;
                }
            }

            if (string.IsNullOrEmpty(title) == false)
            {
                EditorGUILayout.LabelField(title, titleGuiStyle);
            }
            else
            {
                EditorGUILayout.LabelField(string.Empty, titleGuiStyle);
            }

            // drawing the foldout
            Rect foldoutPosition = position;
            foldoutPosition.x += 15;
            foldoutPosition.y += 3;
            foldoutPosition.width = 15;
            foldoutPosition.height = 15;

            foldouts[foldoutId] = EditorGUI.Foldout(foldoutPosition, foldouts[foldoutId], GUIContent.none, false);
        }
    }
}
