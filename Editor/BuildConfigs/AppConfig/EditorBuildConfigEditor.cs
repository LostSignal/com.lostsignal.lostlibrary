//-----------------------------------------------------------------------
// <copyright file="EditorBuildConfigEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(EditorBuildConfigs))]
    public class EditorBuildConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Show Editor"))
            {
                BuildConfigEditorWindow.ShowWindow();
            }
        }
    }
}
