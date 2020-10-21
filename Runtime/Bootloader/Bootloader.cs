//-----------------------------------------------------------------------
// <copyright file="Bootloader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public delegate void BootloaderDelegate();

    public delegate void BootloaderProgressUpdatedDelegate(float progress);

    public delegate void BootloaderProgressTextUpdatedDelegate(string text);

    public class Bootloader : MonoBehaviour
    {
        private static readonly string BootloaderSceneName = "Bootloader";
        private static bool isBootloaderInitialized;

        private static event BootloaderDelegate onBooted;

#pragma warning disable 0649
        [SerializeField] private LazyScene startupScene;

        [Header("Loading UI")]
        [SerializeField] private bool dontShowLoadingInEditor = true;
        [SerializeField] private float minimumLoadingDialogTime;
        [SerializeField] private Camera loadingCamera;
#pragma warning restore 0649

        public static event BootloaderDelegate OnBoot
        {
            add
            {
                if (isBootloaderInitialized)
                {
                    value?.Invoke();
                }

                onBooted += value;
            }

            remove
            {
                onBooted -= value;
            }
        }

        public static event BootloaderDelegate OnReboot;

        public static event BootloaderProgressUpdatedDelegate ProgressUpdated;

        public static event BootloaderProgressTextUpdatedDelegate ProgressTextUpdate;

        public static void UpdateLoadingText(string newText)
        {
            ProgressTextUpdate?.Invoke(newText);
        }

        public static void Reboot()
        {
            OnReboot?.Invoke();
            isBootloaderInitialized = false;
            SceneManager.LoadScene(BootloaderSceneName, LoadSceneMode.Single);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeOnLoad()
        {
            bool bootloaderAlreadyLoaded = false;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == BootloaderSceneName)
                {
                    bootloaderAlreadyLoaded = true;
                }

                // Special case for not loading the bootloader with some scenes
                if (Application.isEditor &&
                    SceneManager.GetSceneAt(i).path.ToUpperInvariant()
                        .Replace(" ", string.Empty)
                        .Replace("_", string.Empty)
                        .Replace("-", string.Empty)
                        .Replace(".", string.Empty)
                        .Contains("NOBOOTLOADER"))
                {
                    return;
                }
            }

            if (bootloaderAlreadyLoaded == false)
            {
                SceneManager.LoadSceneAsync(BootloaderSceneName, LoadSceneMode.Additive);
            }
        }

        private bool ShowLoadingInEditor => Application.isEditor == false || this.dontShowLoadingInEditor == false;

        private void Start()
        {
            this.StartCoroutine(this.Bootup());
        }

        private void OnDestroy()
        {
            if (this.ShowLoadingInEditor)
            {
                this.startupScene.Release();
            }
        }

        private IEnumerator Bootup()
        {
            while (DialogManager.IsInitialized == false)
            {
                yield return null;
            }

            var laodingDialog = DialogManager.GetDialog<BootloaderDialog>().Dialog;

            if (this.ShowLoadingInEditor)
            {
                laodingDialog.Show();
            }

            float startTime = Time.realtimeSinceStartup;

            // Getting all the managers and waiting for them to finish initializing
            List<IManager> managers = new List<IManager>(this.GetComponentsInChildren<IManager>());
            List<IManager> managersToRemove = new List<IManager>(managers.Count);
            int initialManagersCount = managers.Count;

            while (managers.Count > 0)
            {
                managersToRemove.Clear();

                foreach (var manager in managers)
                {
                    if (manager.IsManagerInitialized())
                    {
                        managersToRemove.Add(manager);
                    }
                }

                foreach (var managerToRemvoe in managersToRemove)
                {
                    managers.Remove(managerToRemvoe);
                }

                ProgressUpdated?.Invoke(1.0f - (managers.Count / (float)initialManagersCount));

                yield return null;
            }

            // If the only scene open is the bootloader scene, then lets load the startup scene
            if (SceneManager.sceneCount == 1 && this.startupScene != null && this.startupScene.AssetGuid.IsNullOrWhitespace() == false)
            {
                var loadScene = this.startupScene.LoadScene(LoadSceneMode.Additive);
                yield return loadScene;
                SceneManager.SetActiveScene(loadScene.Result.Scene);
            }

            // Destorying the Loading camera now that the startup scene is loaded (the loading dialog will find the new camera automatically)
            GameObject.DestroyImmediate(this.loadingCamera.gameObject);
            DialogManager.UpdateAllDialogCameras();

            // Making sure we wait the minimum time
            if (this.ShowLoadingInEditor)
            {
                float elapsedTime = Time.realtimeSinceStartup - startTime;

                if (elapsedTime < this.minimumLoadingDialogTime)
                {
                    yield return WaitForUtil.Seconds(this.minimumLoadingDialogTime - elapsedTime);
                }

                // Making sure we don't say Hide if we're still showing (has a bad pop)
                while (laodingDialog.IsShown == false)
                {
                    yield return null;
                }
            }

            // We're done!  Fire the OnBooted event
            isBootloaderInitialized = true;
            onBooted?.Invoke();

            // Doing a little cleanup before giving user control
            System.GC.Collect();
            yield return null;

            if (this.ShowLoadingInEditor)
            {
                laodingDialog.Hide();
            }
        }
    }
}
