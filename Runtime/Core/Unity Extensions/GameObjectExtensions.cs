﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="GameObjectExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class GameObjectExtensions
    {
        public static void SafeSetActive(this GameObject gameObject, bool active)
        {
            if (gameObject && gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        public static Component GetOrAddComponent(this GameObject gameObject, System.Type componentType)
        {
            var component = gameObject.GetComponent(componentType);

            if (component == null)
            {
                component = gameObject.AddComponent(componentType);
            }

            return component;
        }

        public static List<T> GetOrAddComponents<T>(this GameObject gameObject, int count) where T : Component
        {
            List<T> results = new List<T>(gameObject.GetComponents<T>());

            int needed = count - results.Count;
            for (int i = 0; i < needed; i++)
            {
                T component = gameObject.AddComponent<T>();
                results.Add(component);
            }

            return results;
        }

        public static GameObject GetChild(this GameObject gameObject, string name)
        {
            Transform childTransform = gameObject.transform.Find(name);
            return childTransform == null ? null : childTransform.gameObject;
        }

        /// <summary>
        /// Creates a child game object with the given name attached to this GameObject, or finds the game object
        /// if it already exists.  It will make sure that one of each of the given component types exist on the child.
        /// It only makes sure 1 component of that type exists though.  If you need multiple components then use the
        /// GetOrAddComponents method on the returned game object.
        /// </summary>
        /// <param name="gameObject">The game object to search.</param>
        /// <param name="name">The name of the child to search for.</param>
        /// <param name="components">The list of components to add ensure exist on the child.</param>
        /// <returns>The newly created or the found child.</returns>
        public static GameObject GetOrCreateChild(this GameObject gameObject, string name, params System.Type[] components)
        {
            Transform childTransform = gameObject.transform.Find(name);

            // getting/creating a ball object
            if (childTransform == null)
            {
                var childGameObject = components == null || components.Length == 0 ? new GameObject(name) : new GameObject(name, components);
                childGameObject.transform.SetParent(gameObject.transform);
                childGameObject.transform.Reset();

                return childGameObject;
            }
            else
            {
                var childGameObject = childTransform.gameObject;

                if (components != null)
                {
                    foreach (var component in components)
                    {
                        childGameObject.GetOrAddComponent(component);
                    }
                }

                return childGameObject;
            }
        }

        public static void SetLayerRecursively(this GameObject gameObject, string layerName)
        {
            PrivateSetLayerRecursively(gameObject, LayerMask.NameToLayer(layerName));
        }

        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            PrivateSetLayerRecursively(gameObject, layer);
        }

        public static void DestroyChildren(this GameObject gameObject)
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                Pooler.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyAllChildrenRecursively(this GameObject gameObject)
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                PrivateDestroyAllRecursively(gameObject.transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyAllRecursively(this GameObject gameObject)
        {
            PrivateDestroyAllRecursively(gameObject);
        }

        private static void PrivateSetLayerRecursively(GameObject gameObject, int layer)
        {
            if (gameObject.layer != layer)
            {
                gameObject.layer = layer;
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                PrivateSetLayerRecursively(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }

        private static void PrivateDestroyAllRecursively(GameObject gameObject)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                PrivateDestroyAllRecursively(gameObject.transform.GetChild(i).gameObject);
            }

            Pooler.DestroyImmediate(gameObject);
        }
    }
}

#endif
