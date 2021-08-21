#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="FlagPropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(FlagCollection.Flag))]
    public class FlagPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var idRect = new Rect(position.x, position.y, 40, position.height - 2);
            var nameRect = new Rect(position.x + 45, position.y, position.width - 65, position.height - 2);
            var enabledRect = new Rect(position.x + position.width - 15, position.y, 15, position.height - 2);

            // Find Properties
            var idProp = property.FindPropertyRelative("id");
            var nameProp = property.FindPropertyRelative("name");
            var enabledProp = property.FindPropertyRelative("enabled");

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            using (new EditorGUI.DisabledScope(enabledProp.boolValue == false))
            {
                EditorGUI.PropertyField(idRect, idProp, GUIContent.none);
                EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
            }

            EditorGUI.PropertyField(enabledRect, enabledProp, GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
