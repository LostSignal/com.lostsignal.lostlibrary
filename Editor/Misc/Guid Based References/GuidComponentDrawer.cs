﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="GuidComponentDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GuidComponent))]
    public class GuidComponentDrawer : Editor
    {
        private GuidComponent guidComp;

        public override void OnInspectorGUI()
        {
            if (guidComp == null)
            {
                guidComp = (GuidComponent)target;
            }

            // Draw label
            EditorGUILayout.LabelField("Guid:", guidComp.GetGuid().ToString());
        }
    }
}
