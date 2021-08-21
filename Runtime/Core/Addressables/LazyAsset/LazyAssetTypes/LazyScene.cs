#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LazyScene.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    #if UNITY
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;
    #endif

    [Serializable]
    public class LazyScene : LazyAsset, ILazyScene
    {
        #if UNITY
        private AsyncOperationHandle operation;

        public AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> LoadScene(LoadSceneMode loadSceneMode)
        {
            var sceneOperation = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(this.RuntimeKey, loadSceneMode);
            this.operation = sceneOperation;
            return sceneOperation;
        }

        public void Release()
        {
            if (this.operation.IsValid() == false)
            {
                Debug.LogWarning("Cannot release a null or unloaded asset.");
                return;
            }

            UnityEngine.AddressableAssets.Addressables.Release(this.operation);
            this.operation = default(AsyncOperationHandle);
        }

        #endif
    }
}
