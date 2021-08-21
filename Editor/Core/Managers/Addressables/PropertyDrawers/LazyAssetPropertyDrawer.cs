﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LazyAssetPropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(LazyAsset), true)]
    public class LazyAssetPropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, UnityEngine.Object> objectCache = new Dictionary<string, UnityEngine.Object>();
        private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetGuid = property.FindPropertyRelative("assetGuid");

            Type type = this.GetType(property);

            var currentValue = this.GetAsset(assetGuid.stringValue);

            var newValue = EditorGUI.ObjectField(position, GetLabel(), currentValue, type, false);

            if (currentValue != newValue)
            {
                assetGuid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newValue));
            }

            string GetLabel()
            {
                string propertyPath = property.propertyPath;

                if (propertyPath.EndsWith("]"))
                {
                    int leftBracketIndex = propertyPath.LastIndexOf("[");
                    int rightBracketIndex = propertyPath.LastIndexOf("]");
                    string indexString = propertyPath.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);

                    return "Element " + indexString;
                }

                return label.text;
            }
        }

        private Type GetType(SerializedProperty property)
        {
            if (typeCache.TryGetValue(property.propertyPath, out Type type) == false)
            {
                Type propertyType = property.GetSerializedPropertyType();
                object propertyObject = Activator.CreateInstance(propertyType);
                LazyAsset lazyAsset = propertyObject as LazyAsset;

                type = lazyAsset.Type;
                typeCache.Add(property.propertyPath, type);
            }

            return type;
        }

        private UnityEngine.Object GetAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            UnityEngine.Object obj;
            if (objectCache.TryGetValue(guid, out obj) == false)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                objectCache.Add(guid, obj);
            }

            return obj;
        }
    }
}
