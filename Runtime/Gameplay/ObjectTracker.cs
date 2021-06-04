//-----------------------------------------------------------------------
// <copyright file="ObjectTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class ObjectTracker : Manager<ObjectTracker>
    {
        private Dictionary<Type, List<object>> objects = new Dictionary<Type, List<object>>();

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        public static void UpdateRegistration<T>(T obj) where T : MonoBehaviour
        {
            if (Instance && obj)
            {
                if (obj.gameObject.activeSelf)
                {
                    ObjectTracker.Instance.Register(obj);
                }
                else
                {
                    ObjectTracker.Instance.Deregister(obj);
                }
            }
        }

        public void Register<T>(T obj) where T : class
        {
            if (obj == null)
            {
                Debug.LogError("ObjectTracker tried to register a NULL object!");
                return;
            }

            Type objectType = typeof(T);

            List<object> objectList = null;

            if (this.objects.TryGetValue(objectType, out objectList))
            {
                if (Debug.isDebugBuild && objectList.Contains(obj))
                {
                    Debug.LogErrorFormat(obj as UnityEngine.Object, "ObjectTracker had an object of type {0} added multiple times!", objectType.Name);
                }
                else
                {
                    objectList.Add(obj);
                }
            }
            else
            {
                this.objects.Add(objectType, new List<object> { obj });
            }
        }

        public void Deregister<T>(T obj) where T : class
        {
            if (obj == null)
            {
                Debug.LogError("ObjectTracker tried to deregister a NULL object!");
                return;
            }

            Type objectType = typeof(T);

            List<object> objectList = null;
            bool success = false;

            if (this.objects.TryGetValue(objectType, out objectList))
            {
                success = objectList.Remove(obj);
            }

            if (success == false)
            {
                Debug.LogErrorFormat(obj as UnityEngine.Object, "ObjectTracker tried to remove object of type {0} before adding it!", objectType.Name);
            }
        }

        public IEnumerable<T> GetObjects<T>() where T : class
        {
            List<object> objectsList = null;

            if (this.objects.TryGetValue(typeof(T), out objectsList))
            {
                for (int i = 0; i < objectsList.Count; i++)
                {
                    yield return objectsList[i] as T;
                }
            }
        }

        public int GetObjectCount<T>() where T : class
        {
            if (this.objects.TryGetValue(typeof(T), out List<object> objectsList))
            {
                return objectsList.Count;
            }

            return 0;
        }

        public T GetFirstObject<T>() where T : class
        {
            List<object> objectsList = null;

            if (this.objects.TryGetValue(typeof(T), out objectsList))
            {
                if (objectsList.Count > 0)
                {
                    return objectsList[0] as T;
                }
            }

            return null;
        }
    }
}

#endif
