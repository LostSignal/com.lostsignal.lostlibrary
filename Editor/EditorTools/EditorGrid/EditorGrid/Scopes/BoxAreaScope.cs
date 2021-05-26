//-----------------------------------------------------------------------
// <copyright file="FoldoutHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.EditorGrid
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class BoxAreaScope : IDisposable
    {
        private static GUIStyle titleGuiStyle = null;

        private static GUIStyle TitleGuiStyle
        {
            get
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

                return titleGuiStyle;
            }
        }

        public BoxAreaScope(string title)
        {
            EditorGUILayout.BeginVertical("box");

            if (string.IsNullOrEmpty(title) == false)
            {
                EditorGUILayout.LabelField(title, TitleGuiStyle);
            }

            EditorGUILayout.Space(5);
        }

        public void Dispose()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }
    }
}
