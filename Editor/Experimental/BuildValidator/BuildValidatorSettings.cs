#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="BuildValidatorSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.BuildConfig;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [BuildConfigSettingsOrder(2000)]
    public class BuildValidatorSettings : BuildConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private bool warningsAsErrors;
#pragma warning restore 0649

        public override string DisplayName => "Build Validator";
        public override bool IsInline => true;

        [EditorEvents.OnProcessScene]
        private static void OnProcessScene(Scene scene, BuildReport report)
        {
            var settings = EditorBuildConfigs.GetActiveSettings<BuildValidatorSettings>();

            if (settings == null)
            {
                return;
            }

            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                ValidateComponents(rootGameObject.GetComponentsInChildren(typeof(Component), true));
            }
        }

        [EditorEvents.OnPreprocessBuild]
        private static void OnPreproccessBuild()
        {
            var settings = EditorBuildConfigs.GetActiveSettings<BuildValidatorSettings>();

            if (settings == null)
            {
                return;
            }

            // Validating Prefabs
            foreach (var prefabPath in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.prefab", SearchOption.AllDirectories))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(prefabPath);

                ValidateComponents(prefab.GetComponentsInChildren(typeof(Component), true));
            }

            // Scriptable Objects
            foreach (var scriptableObjectPath in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.asset", SearchOption.AllDirectories))
            {
                var scriptableObject = AssetDatabase.LoadAssetAtPath(scriptableObjectPath, typeof(UnityEngine.ScriptableObject));
                var serializedObject = new SerializedObject(scriptableObject);

                ValidateSerializedObject(serializedObject);
            }
        }

        private static void ValidateComponents(Component[] components)
        {
            foreach (var component in components)
            {
                SerializedObject serializedObject = new SerializedObject(component);

                ValidateSerializedObject(serializedObject);
            }
        }

        private static void ValidateSerializedObject(SerializedObject serializedObject)
        {
            // Validate this object
            (serializedObject.targetObject as IValidate)?.Validate();

            // Validate all of it's properties
            // Recursively Iterate over all properties (make sure to check if it's an array and iterate over every element if it is)
        }
    }
}
