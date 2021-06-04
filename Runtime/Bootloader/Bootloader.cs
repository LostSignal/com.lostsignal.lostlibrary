//-----------------------------------------------------------------------
// <copyright file="Bootloader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

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
        public enum ManagersLocation
        {
            SceneName,
            // ResourcesPrefabName,
            // AddressablesPrefab,
            // AddressablesScene,
        }

        private static event BootloaderDelegate onManagersReady;
        private static bool areManagersInitialized;
        private static Bootloader bootloaderInstance;

#pragma warning disable 0649
        [SerializeField] private ManagersLocation managersLocation;
        [SerializeField] private string managersSceneName;
        // [SerializeField] private string managersResourcesPrefabName;
        // [SerializeField] private LazyAsset<Bootloader> managersAddressablesPrefab;
        // [SerializeField] private LazyScene managersAddressablesScene;

        [Header("Loading UI")]
        [SerializeField] private bool dontShowLoadingInEditor = true;
        [SerializeField] private float minimumLoadingDialogTime;
        [SerializeField] private Camera loadingCamera;

        #if UNITY_EDITOR
        [Header("Editor Ignore Scenes")]
        [SerializeField] private List<UnityEditor.SceneAsset> scenesToIgnore;
        #endif
#pragma warning restore 0649

        public static event BootloaderDelegate OnReset;

        public static event BootloaderDelegate OnManagersReady
        {
            add
            {
                if (areManagersInitialized)
                {
                    value?.Invoke();
                }

                onManagersReady += value;
            }

            remove
            {
                onManagersReady -= value;
            }
        }

        public static event BootloaderProgressUpdatedDelegate ProgressUpdated;

        public static event BootloaderProgressTextUpdatedDelegate ProgressTextUpdate;

        public static void UpdateLoadingText(string newText)
        {
            ProgressTextUpdate?.Invoke(newText);
        }

        public static void Reboot()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);

            // Destory old bootloader instance
            ResetBootloader();
            BootBootloader();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootBootloader()
        {
            var bootloaderPrefab = Resources.Load<Bootloader>("Bootloader");

            if (ShouldInstantiateBootloader(bootloaderPrefab))
            {
                bootloaderInstance = GameObject.Instantiate(bootloaderPrefab);
                GameObject.DontDestroyOnLoad(bootloaderInstance);
            }
        }

        private static bool ShouldInstantiateBootloader(Bootloader bootloaderPrefab)
        {
            #if UNITY_EDITOR
            var activeSceneName = SceneManager.GetActiveScene().name;
            foreach (var sceneToIgnore in bootloaderPrefab.scenesToIgnore)
            {
                if (activeSceneName == sceneToIgnore.name)
                {
                    return false;
                }
            }
            #endif

            return true;
        }

        [EditorEvents.OnExitPlayMode]
        private static void ResetBootloader()
        {
            if (bootloaderInstance != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(bootloaderInstance.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(bootloaderInstance.gameObject);
                }
            }

            areManagersInitialized = false;
            onManagersReady = null;
            bootloaderInstance = null;

            OnReset?.Invoke();
        }

        // ----------------------------------------------------------------------------------------------------------------

        private bool ShowLoadingInEditor => Application.isEditor == false || this.dontShowLoadingInEditor == false;

        private void Start()
        {
            this.StartCoroutine(this.Bootup());
        }

        private void OnDestroy()
        {
            if (this.ShowLoadingInEditor)
            {
                //this.startupScene.Release();
            }
        }

        private IEnumerator Bootup()
        {
            while (DialogManager.IsInitialized == false)
            {
                yield return null;
            }

            Dialog bootloaderDialog = null;

            if (this.ShowLoadingInEditor)
            {
                bootloaderDialog = DialogManager.GetDialog<BootloaderDialog>().Dialog;
                bootloaderDialog.Show();
            }

            while (ReleasesManager.IsInitialized == false || XRManager.IsInitialized == false)
            {
                yield return null;
            }

            // TODO [bgish]: Initialize Addressables based on ReleaseManager.CurrentRelease

            yield return this.StartManagers();

            float startTime = Time.realtimeSinceStartup;

            // Getting all the managers and waiting for them to finish initializing
            List<IManager> managers = new List<IManager>(ManagerList.Managers);
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

            //// // If the only scene open is the bootloader scene, then lets load the startup scene
            //// if (SceneManager.sceneCount == 1 && this.startupScene != null && this.startupScene.AssetGuid.IsNullOrWhitespace() == false)
            //// {
            ////     var loadScene = this.startupScene.LoadScene(LoadSceneMode.Additive);
            ////     yield return loadScene;
            ////     SceneManager.SetActiveScene(loadScene.Result.Scene);
            //// }

            // Destorying the Loading camera now that the startup scene is loaded (the loading dialog will find the new camera automatically)
            GameObject.DestroyImmediate(this.loadingCamera.gameObject);
            DialogManager.UpdateAllDialogCameras();

            // Making sure we wait the minimum time
            if (this.ShowLoadingInEditor && bootloaderDialog)
            {
                float elapsedTime = Time.realtimeSinceStartup - startTime;

                if (elapsedTime < this.minimumLoadingDialogTime)
                {
                    yield return WaitForUtil.Seconds(this.minimumLoadingDialogTime - elapsedTime);
                }

                // Making sure we don't say Hide if we're still showing (has a bad pop)
                while (bootloaderDialog.IsShown == false)
                {
                    yield return null;
                }
            }

            // We're done!  Fire the OnBooted event
            areManagersInitialized = true;

            Debug.Log("Bootloader.OnBooted()");
            onManagersReady?.Invoke();

            // Doing a little cleanup before giving user control
            System.GC.Collect();
            yield return null;

            if (this.ShowLoadingInEditor && bootloaderDialog)
            {
                bootloaderDialog.Hide();
            }
        }

        private IEnumerator StartManagers()
        {
            switch (this.managersLocation)
            {
                case ManagersLocation.SceneName:
                    yield return SceneManager.LoadSceneAsync(this.managersSceneName, LoadSceneMode.Additive);
                    break;

                default:
                    Debug.LogError($"Unknown ManagerLocation {this.managersLocation} Found!");
                    break;
            }
        }
    }
}

#endif
