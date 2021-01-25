//-----------------------------------------------------------------------
// <copyright file="XRInteractionHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace HavenXR
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.XR.Interaction.Toolkit;

    public static class XRInteractionHelper
    {
        private static XRInteractionManager xrInteractionManagerInstance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Create()
        {
            xrInteractionManagerInstance = GameObject.FindObjectOfType<XRInteractionManager>();

            if (xrInteractionManagerInstance == null)
            {
                xrInteractionManagerInstance = new GameObject("XRInteractionManager", typeof(XRInteractionManager)).GetComponent<XRInteractionManager>();
                GameObject.DontDestroyOnLoad(xrInteractionManagerInstance.gameObject);
            }

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FixTeleports();
        }

        private static void FixTeleports()
        {
            xrInteractionManagerInstance.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return HavenRig.WaitForRig();

                var rig = HavenRig.GetRig();
                var teleportProvider = rig.GetComponentInChildren<TeleportationProvider>();

                foreach (var teleport in GameObject.FindObjectsOfType<BaseTeleportationInteractable>(true))
                {
                    teleport.interactionManager = xrInteractionManagerInstance;
                    teleport.teleportationProvider = teleportProvider;
                }
            }
        }
    }
}

#endif
