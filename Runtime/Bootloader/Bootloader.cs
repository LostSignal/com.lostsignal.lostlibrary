//-----------------------------------------------------------------------
// <copyright file="Bootloader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using Lost.BuildConfig;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public delegate void BootloaderDelegate();

    public delegate void BootloaderProgressUpdatedDelegate(float progress);

    public delegate void BootloaderProgressTextUpdatedDelegate(string text);

    public class Bootloader : MonoBehaviour
    {
        // RuntimeConfig Settings Keys
        public const string BootloaderResourcePathSetting = "Bootloader.ResourcePath";
        public const string BootloaderRebootSceneName = "Bootloader.RebootSceneName";
        public const string BootloaderManagersLocation = "Bootloader.ManagersLocation";
        public const string BootloaderManagersPath = "Bootloader.ManagersPath";

        // Default Values
        public const string LostBootloaderResourcePath = "Lost/Bootloader";
        public const string LostManagersResourcePath = "Lost/Managers";
        public const string DefaultRebootSceneName = "Main";

        public enum ManagersLocation
        {
            ResourcesPath,
            SceneName,
            //// AddressablesPrefab,
            //// AddressablesScene,
        }

        private static event BootloaderDelegate onManagersReady;
        private static bool areManagersInitialized;
        private static Bootloader bootloaderInstance;
        private static GameObject managersInstance;

#pragma warning disable 0649
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
            string rebootSceneName = RuntimeBuildConfig.Instance.GetString(Bootloader.BootloaderRebootSceneName);

            SceneManager.LoadScene(rebootSceneName, LoadSceneMode.Single);

            // Destory old bootloader instance
            ResetBootloader();
            BootBootloader();
        }

        public static bool AreManagersReady
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            get => areManagersInitialized;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootBootloader()
        {
            string bootloaderResourcePath = RuntimeBuildConfig.Instance.GetString(BootloaderResourcePathSetting);
            string managersLocation = RuntimeBuildConfig.Instance.GetString(BootloaderManagersLocation);
            string managersPath = RuntimeBuildConfig.Instance.GetString(BootloaderManagersPath);
            string rebootSceneName = RuntimeBuildConfig.Instance.GetString(Bootloader.BootloaderRebootSceneName);

            if (string.IsNullOrWhiteSpace(bootloaderResourcePath) ||
                string.IsNullOrWhiteSpace(managersLocation) ||
                string.IsNullOrWhiteSpace(managersPath) ||
                string.IsNullOrWhiteSpace(rebootSceneName))
            {
                Debug.LogError("Failed To Boot - Bootloader and/or Manager info is Empty! Do you have an active build config with the Bootloader Settings added?");
                return;
            }

            var bootloaderPrefab = Resources.Load<Bootloader>(bootloaderResourcePath);

            if (bootloaderPrefab == null)
            {
                Debug.LogError($"Failed To Boot - Unable to load Bootloader Prefab \"{bootloaderResourcePath}\".");
                return;
            }

            if (ShouldInstantiateBootloader(bootloaderPrefab))
            {
                bootloaderInstance = GameObject.Instantiate(bootloaderPrefab);
                bootloaderInstance.name = bootloaderInstance.name.Replace("(Clone)", string.Empty);
                GameObject.DontDestroyOnLoad(bootloaderInstance);
            }

            bool ShouldInstantiateBootloader(Bootloader bootloaderPrefab)
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

            if (managersInstance != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(managersInstance);
                }
                else
                {
                    GameObject.DestroyImmediate(managersInstance);
                }
            }

            areManagersInitialized = false;
            onManagersReady = null;
            bootloaderInstance = null;
            managersInstance = null;

            OnReset?.Invoke();
        }

        //// ----------------------------------------------------------------------------------------------------------------

        private bool ShowLoadingInEditor => Application.isEditor == false || this.dontShowLoadingInEditor == false;

        private void Start()
        {
            this.StartCoroutine(this.Bootup());
        }

        private void OnDestroy()
        {
            //// Clean up lazy assets only if they were instantiated
            //// this.managersAddressablesPrefab.Release();
            //// this.managersAddressablesScene.Release();
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

            yield return ReleasesManager.Instance.ShowForceUpdateDialog();

            while (AddressablesManager.IsInitialized == false)
            {
                yield return null;
            }

            yield return this.StartManagers();

            float startTime = Time.realtimeSinceStartup;

            yield return WaitForManagersToInitialize();

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
            onManagersReady?.Invoke();
            onManagersReady = null;

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
            if (int.TryParse(RuntimeBuildConfig.Instance.GetString(BootloaderManagersLocation), out int maangersLocationIntValue) == false)
            {
                Debug.LogError($"Unable to startup Managers.  ManagerLocation was not a valid int \"{BootloaderManagersLocation}\"", this);
                yield break;
            }

            var managersLocation = (ManagersLocation)maangersLocationIntValue;
            var managersPath = RuntimeBuildConfig.Instance.GetString(BootloaderManagersPath);

            switch (managersLocation)
            {
                case ManagersLocation.ResourcesPath:
                    {
                        managersInstance = GameObject.Instantiate(Resources.Load<GameObject>(managersPath));
                        managersInstance.name = managersInstance.name.Replace("(Clone)", string.Empty);
                        DontDestroyOnLoad(managersInstance);
                        break;
                    }

                case ManagersLocation.SceneName:
                    {
                        bool sceneAlreadyLoaded = false;

                        for (int i = 0; i < SceneManager.sceneCount; i++)
                        {
                            sceneAlreadyLoaded |= SceneManager.GetSceneAt(i).name == managersPath;
                        }

                        if (sceneAlreadyLoaded == false)
                        {
                            yield return SceneManager.LoadSceneAsync(managersPath, LoadSceneMode.Additive);
                        }
                        
                        break;
                    }

                default:
                    Debug.LogError($"Unknown ManagerLocation {managersLocation} Found!");
                    break;
            }
        }

        private IEnumerator WaitForManagersToInitialize()
        {
            int initialManagersCount = ManagerTracker.Managers.Count;
            List<IManager> managers = new List<IManager>(initialManagersCount);
            List<IManager> managersToRemove = new List<IManager>(initialManagersCount);

            // Populating the manager list with all known managers
            for (int i = 0; i < initialManagersCount; i++)
            {
                managers.Add(ManagerTracker.Managers[i]);
            }

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
        }
    }
}

#endif
