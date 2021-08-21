#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="HavenTeleportEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_HAVEN

namespace Lost.Haven
{
    using UnityEditor;
    using UnityEditor.XR.Interaction.Toolkit;

    [CustomEditor(typeof(HavenTeleport), true), CanEditMultipleObjects]
    public class HavenTeleportEditor : BaseTeleportationInteractableEditor
    {
        private SerializedProperty teleportType;
        private SerializedProperty anchorOverrideTransform;
        private SerializedProperty colliders;
        private SerializedProperty customReticle;
        private SerializedProperty setScaleOnTeleport;
        private SerializedProperty rigScale;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.teleportType = serializedObject.FindProperty("type");
            this.anchorOverrideTransform = serializedObject.FindProperty("anchorOverrideTransform");
            this.colliders = serializedObject.FindProperty("m_Colliders");
            this.customReticle = serializedObject.FindProperty("m_CustomReticle");
            this.setScaleOnTeleport = serializedObject.FindProperty("setScaleOnTeleport");
            this.rigScale = serializedObject.FindProperty("rigScale");
        }

        protected override void DrawProperties()
        {
            EditorGUILayout.PropertyField(this.teleportType);

            if (this.teleportType.intValue == (int)HavenTeleport.TeleportType.Anchor)
            {
                EditorGUILayout.PropertyField(this.anchorOverrideTransform);
            }

            EditorGUILayout.PropertyField(this.colliders);
            EditorGUILayout.PropertyField(this.customReticle);
            EditorGUILayout.PropertyField(this.setScaleOnTeleport);

            if (this.setScaleOnTeleport.boolValue)
            {
                EditorGUILayout.PropertyField(this.rigScale);
            }
        }
    }
}

#endif
