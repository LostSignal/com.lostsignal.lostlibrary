//-----------------------------------------------------------------------
// <copyright file="GameObjectStateEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lost.EditorGrid;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GameObjectState))]
    public class GameObjectStateEditor : Editor
    {
        private static readonly List<Type> ActionTypes;
        private static readonly string[] ActionTypeNames;
        private static int selectedType;

        static GameObjectStateEditor()
        {
            ActionTypes = TypeCache.GetTypesDerivedFrom<GameObjectStateAction>().ToList();
            ActionTypeNames = ActionTypes.Select(x => x.Name).ToArray();
            selectedType = 0;
        }

        public override void OnInspectorGUI()
        {
            var states = this.serializedObject.FindProperty("states");

            this.DrawStates(states);
            this.AddStateButton(states);

            // Saving if object changed
            if (this.serializedObject.hasModifiedProperties)
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawStates(SerializedProperty states)
        {
            for (int i = 0; i < states.arraySize; i++)
            {
                this.DrawState(i, states.GetArrayElementAtIndex(i), out bool deleted);

                if (deleted)
                {
                    states.DeleteArrayElementAtIndex(i);
                    break;
                }
            }
        }

        private void AddStateButton(SerializedProperty states)
        {
            if (GUILayout.Button("Add State"))
            {
                int newIndex = states.arraySize;
                states.InsertArrayElementAtIndex(newIndex);

                var newState = states.GetArrayElementAtIndex(newIndex);
                newState.FindPropertyRelative("name").stringValue = "New State " + newIndex;
            }
        }

        private void DrawState(int index, SerializedProperty state, out bool deleted)
        {
            var nameProperty = state.FindPropertyRelative("name");

            using (new FoldoutScope(this.target.GetInstanceID() + index, nameProperty.stringValue, out bool visible, out Rect position))
            {
                // Delete State Button
                if (GUI.Button(new Rect(position.x + position.width - 17, position.y + 6, 12, 12), ButtonUtil.DeleteTexture, GUIStyle.none))
                {
                    deleted = true;
                    return;
                }

                // Switch To State Button
                if (Application.isPlaying && GUI.Button(new Rect(position.x + position.width - 72, position.y + 5, 50, 15), "Switch"))
                {
                    var gameObjectState = this.target as GameObjectState;
                    gameObjectState.SetState(nameProperty.stringValue);
                    deleted = false;
                    return;
                }

                if (visible)
                {
                    this.DrawStateOpen(state, nameProperty);
                }
            }

            deleted = false;
        }

        private void DrawStateOpen(SerializedProperty state, SerializedProperty nameProperty)
        {
            var actions = state.FindPropertyRelative("actions");

            int labelWidth = 60;

            using (new LabelWidthScope(labelWidth))
            {
                EditorGUILayout.PropertyField(nameProperty);
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Action", GUILayout.Width(labelWidth));

                selectedType = EditorGUILayout.Popup(selectedType, ActionTypeNames);

                if (GUILayout.Button("Add", GUILayout.Width(40)))
                {
                    var action = Activator.CreateInstance(ActionTypes[selectedType]) as GameObjectStateAction;
                    actions.arraySize++;
                    actions.GetArrayElementAtIndex(actions.arraySize - 1).managedReferenceValue = action;
                }
            }

            var gameObjectState = this.target as GameObjectState;

            for (int i = 0; i < actions.arraySize; i++)
            {
                var editorName = gameObjectState.GetEditorDisplayName(nameProperty.stringValue, i);

                //// NOTE [bgish]: I used to only display the type name
                //// string typeName = actions.GetArrayElementAtIndex(i).type
                ////     .Replace("managedReference<", string.Empty)
                ////     .Replace(">", string.Empty);

                using (new IndentLevelScope(1))
                {
                    EditorGUILayout.PropertyField(actions.GetArrayElementAtIndex(i), new GUIContent(editorName), true);
                }
            }
        }
    }
}
