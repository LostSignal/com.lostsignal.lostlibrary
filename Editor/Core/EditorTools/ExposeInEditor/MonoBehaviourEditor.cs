//-----------------------------------------------------------------------
// <copyright file="MonoBehaviourEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawEditorMethods(this.target);
        }

        public static void DrawEditorMethods(UnityEngine.Object obj)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            foreach (var method in obj.GetType().GetMethods(bindingFlags))
            {                                
                foreach (var attribute in method.GetCustomAttributes(true).OfType<ShowEditorInfoAttribute>())
                {
                    ShowHelpBox(attribute.Text, method, MessageType.Info);
                }
                                
                foreach (var attribute in method.GetCustomAttributes(true).OfType<ShowEditorWarningAttribute>())
                {
                    ShowHelpBox(attribute.Text, method, MessageType.Warning);
                }
                                
                foreach (var attribute in method.GetCustomAttributes(true).OfType<ShowEditorErrorAttribute>())
                {
                    ShowHelpBox(attribute.Text, method, MessageType.Error);
                }
                
                foreach (var attribute in method.GetCustomAttributes(true).OfType<ExposeInEditorAttribute>())
                {
                    if (GUILayout.Button(attribute.Name))
                    {
                        method.Invoke(obj, null);
                    }
                }
            }

            void ShowHelpBox(string text, MethodInfo method, MessageType messageType)
            {
                string displayText = text;

                if (string.IsNullOrWhiteSpace(displayText))
                {
                    displayText = method.Invoke(obj, null) as string;
                }

                if (string.IsNullOrWhiteSpace(displayText) == false)
                {
                    EditorGUILayout.HelpBox(displayText, messageType);
                }
            }
        }
    }
}
