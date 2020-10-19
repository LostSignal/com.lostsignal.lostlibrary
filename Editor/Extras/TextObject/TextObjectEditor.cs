//-----------------------------------------------------------------------
// <copyright file="TextObjectEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TextObject))]
    public class TextObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var textObject = this.target as TextObject;
            textObject.Text = EditorGUILayout.TextArea(textObject.Text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }
    }
}
