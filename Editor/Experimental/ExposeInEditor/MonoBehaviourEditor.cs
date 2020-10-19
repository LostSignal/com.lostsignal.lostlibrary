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
                foreach (var attribute in method.GetCustomAttributes(true).OfType<ExposeInEditorAttribute>())
                {
                    if (GUILayout.Button(attribute.Name))
                    {
                        method.Invoke(obj, null);
                    }
                }
            }
        }
    }
}
