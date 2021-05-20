//-----------------------------------------------------------------------
// <copyright file="EditorAppConfigEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(EditorAppConfig))]
    public class EditorAppConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Show Editor"))
            {
                AppConfigEditorWindow.ShowWindow();
            }
        }
    }
}
