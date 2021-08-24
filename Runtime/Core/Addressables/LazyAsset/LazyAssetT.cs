//-----------------------------------------------------------------------
// <copyright file="LazyAssetT.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    #if UNITY

    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "Using Unity Serialization")]
    [Serializable]
    public class LazyAssetT<T> : LazyAsset, ILazyAsset, IValidate
        where T : UnityEngine.Object
    {
        private AsyncOperationHandle operation;
        private UnityTask<T> cachedTask;

        #if UNITY_EDITOR
        private T cachedEditorAsset;
        #endif

        public LazyAssetT()
        {
        }

        public LazyAssetT(string guid)
            : base(guid)
        {
        }

        public override Type Type
        {
            get { return typeof(T); }
        }

        public bool IsLoaded => this.operation.IsValid() && this.operation.IsDone;

        #if UNITY_EDITOR
        [JsonIgnore]
        public virtual T EditorAsset
        {
            get
            {
                if (string.IsNullOrEmpty(this.AssetGuid))
                {
                    return null;
                }

                // Checking if the asset guid has changed (thus invalidating the cached object)
                if (this.cachedEditorAsset != null)
                {
                    string cachedGuid = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(this.cachedEditorAsset));

                    if (cachedGuid != this.AssetGuid)
                    {
                        this.cachedEditorAsset = null;
                    }
                }

                if (this.cachedEditorAsset == null)
                {
                    var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(this.AssetGuid);
                    this.cachedEditorAsset = (T)UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                }

                return this.cachedEditorAsset;
            }
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "I specifically check for Type Component before calling.")]
        public UnityTask<T> Load()
        {
            #if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(GameObject))
            {
                Debug.LogWarningFormat("You are loading LastAsset<{0}> as if it were a resource, you should instead use Instantiate instead of Load.", typeof(T).Name);
            }
            #endif

            if (this.cachedTask != null)
            {
                return this.cachedTask;
            }
            else
            {
                return UnityTask<T>.Run(Coroutine());
            }

            IEnumerator<T> Coroutine()
            {
                if (typeof(T) == typeof(Sprite))
                {
                    this.operation = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Sprite>(this.RuntimeKey);
                }
                else
                {
                    this.operation = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<UnityEngine.Object>(this.RuntimeKey);
                }

                while (this.operation.IsDone == false && this.operation.Status != AsyncOperationStatus.Failed)
                {
                    yield return default;
                }

                if (this.operation.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogErrorFormat("Unable to successfully load asset {0} of type {1}", this.AssetGuid, typeof(T).Name);
                    yield return default;
                    yield break;
                }

                T value;

                if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    var gameObject = this.operation.Result as GameObject;

                    if (gameObject == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} is not of type GameObject, so can't get Component {1} from it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }

                    value = gameObject.GetComponent<T>();

                    if (value == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} does not have Component {1} on it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }
                }
                else
                {
                    value = this.operation.Result as T;

                    if (value == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} is not of type {1}.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }
                }

                this.cachedTask = UnityTask<T>.Empty(value);
                yield return value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "I specifically check for Type Component before calling.")]
        public UnityTask<T> Instantiate(Transform parent = null, bool reset = true)
        {
            #if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(Component)) == false && typeof(T) != typeof(GameObject))
            {
                Debug.LogWarningFormat("You are Instantiating LastAsset<{0}> as if it were a GameObject, you should instead use Load instead of Instantiate.", typeof(T).Name);
            }
            #endif

            return UnityTask<T>.Run(Coroutine());

            IEnumerator<T> Coroutine()
            {
                var instantiateOperation = UnityEngine.AddressableAssets.Addressables.InstantiateAsync(this.RuntimeKey, parent);

                while (instantiateOperation.IsDone == false && instantiateOperation.Status != AsyncOperationStatus.Failed)
                {
                    yield return default;
                }

                if (instantiateOperation.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogErrorFormat("Unable to successfully instantiate asset {0} of type {1}", this.AssetGuid, typeof(T).Name);
                    yield return default;
                    yield break;
                }

                var gameObject = instantiateOperation.Result;

                if (gameObject != null && reset)
                {
                    gameObject.transform.Reset();
                }

                if (typeof(T) == typeof(GameObject))
                {
                    yield return gameObject as T;
                }
                else if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    if (gameObject == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} is not of type GameObject, so can't get Component {1} from it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }

                    var component = gameObject.GetComponent<T>();

                    if (component == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} does not have Component {1} on it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }

                    yield return component;
                }
                else
                {
                    Debug.LogError("LazyAssetT hit unknown if/else situtation.");
                }
            }
        }

        public void Release()
        {
            if (this.operation.IsValid() == false)
            {
                Debug.LogWarning("Cannot release a null or unloaded asset.");
                return;
            }

            UnityEngine.AddressableAssets.Addressables.Release(this.operation);
            this.operation = default;
            this.cachedTask = null;
        }

        void IValidate.Validate()
        {
            #if UNITY_EDITOR
            // TODO [bgish]: Verify Guid is valid and the object can be cast as T
            throw new NotImplementedException();
            #endif
        }
    }

    #else

    [System.Serializable]
    public class LazyAsset<T> : LazyAsset, ILazyAsset
    {
    }

    #endif
}
