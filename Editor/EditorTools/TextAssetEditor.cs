//-----------------------------------------------------------------------
// <copyright file="TextAssetEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TextAsset))]
    public class TextAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // var textAsset = this.target as TextAsset;
            // textAsset.text = EditorGUILayout.TextArea(textAsset.text);
            // textAsset.
        }
    }
}
