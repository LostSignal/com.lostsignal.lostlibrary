//-----------------------------------------------------------------------
// <copyright file="GPSManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    // https://stackoverflow.com/questions/53046670/how-to-get-values-from-methods-written-in-ios-plugin-in-unity
    // https://medium.com/@nosuchstudio/how-to-access-gps-location-in-unity-521f1371a7e3
    public sealed class GPSManager : Manager<GPSManager>
    {
#pragma warning disable 0649
        [SerializeField] private GPSAccuracy accuracy;
        [SerializeField] private bool waitForUnityRemote;
        [SerializeField] private bool disableGpsWhenLostFocus;
        [SerializeField] private float updateFrequencyInSeconds = 1.0f;

        //// [Header("Force User To Use GPS")]
        //// [SerializeField] private bool appMustUseGps = false;
        //// [SerializeField] private string mustTurnOnGpsMessage = string.Empty; // TODO [bgish]: Make localized string

        [Header("Debug")]
        [SerializeField] private List<DebugStartLocation> debugStartLocations;
#pragma warning restore 0649

        private GPSServiceState serviceState;
        private Coroutine serviceCoroutine;

        public enum GPSServiceState
        {
            Stopped,
            StartingUp,
            Running,
        }

        public enum GPSAccuracy
        {
            Fine,
            Coarse,
        }

        public bool IsRunning => this.serviceState == GPSServiceState.Running;

        public bool IsStartingUp => this.serviceState == GPSServiceState.StartingUp;

        public bool IsStopped => this.serviceState == GPSServiceState.Stopped;

        public bool IsPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.Android ||
                    Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.isEditor;
            }
        }

        public override void Initialize()
        {
            this.EnsurePlayerSettingsAreCorrect();
            this.SetInstance(this);
        }

        public void StartGpsService()
        {
            this.StopGpsService();
            this.serviceCoroutine = CoroutineRunner.Instance.StartCoroutine(this.StartGpsServiceCoroutine());
        }

        public void StopGpsService()
        {
            if (this.serviceCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(this.serviceCoroutine);
                this.serviceCoroutine = null;
            }

            UnityEngine.Input.location.Stop();
            this.serviceState = GPSServiceState.Stopped;
        }

        private IEnumerator StartGpsServiceCoroutine()
        {
            this.serviceState = GPSServiceState.StartingUp;

            if (Application.isEditor && this.waitForUnityRemote)
            {
#if UNITY_EDITOR
                while (UnityEditor.EditorApplication.isRemoteConnected == false)
                {
                    yield return null;
                }
#endif

                yield return WaitForUtil.Seconds(5.0f);
            }

            GPSUtil.AskForPermissionToUseGPS();

            if (GPSUtil.IsGpsEnabledByUser() == false)
            {
                Debug.LogError("Unable to start GPS Manage.  The user doesn't have permissions.");

                this.StopGpsService();
                yield break;
            }

            // Start service before querying location
            UnityEngine.Input.location.Start(10.0f, 10.0f);

            // Wait until service initializes
            int maxWait = 15;
            while (UnityEngine.Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                maxWait--;
            }

            // Editor has a bug which doesn't set the service status to Initializing. So extra wait in Editor.
            if (Application.isEditor)
            {
                int editorMaxWait = 15;
                while (UnityEngine.Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0)
                {
                    yield return new WaitForSecondsRealtime(1);
                    editorMaxWait--;
                }
            }

            // Service didn't initialize in 15 seconds
            if (maxWait < 1)
            {
                // TODO Failure
                Debug.LogFormat("Timed out");
                this.StopGpsService();
                yield break;
            }

            // Connection has failed
            if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
            {
                // TODO Failure
                Debug.LogFormat("Unable to determine device location. Failed with status {0}", UnityEngine.Input.location.status);
                this.StopGpsService();
                yield break;
            }
            else
            {
                this.serviceState = GPSServiceState.Running;

                while (true)
                {
                    Debug.LogFormat("Location: "
                        + UnityEngine.Input.location.lastData.latitude + " "
                        + UnityEngine.Input.location.lastData.longitude + " "
                        + UnityEngine.Input.location.lastData.altitude + " "
                        + UnityEngine.Input.location.lastData.horizontalAccuracy + " "
                        + UnityEngine.Input.location.lastData.timestamp);

                    yield return WaitForUtil.Seconds(this.updateFrequencyInSeconds);
                }
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (this.disableGpsWhenLostFocus == false)
            {
                return;
            }

            if (focus)
            {
                this.StartGpsService();
            }
            else
            {
                this.StopGpsService();
            }
        }

        private void EnsurePlayerSettingsAreCorrect()
        {
#if UNITY_EDITOR && UNITY_ANDROID
            // TODO [bgish]: Turn on "Low Accuracy Location" player setting if it's off
#endif

#if UNITY_EDITOR && UNITY_IOS
            // TODO [bgish]: Set "Location Usage Description" if null or empty
            // UnityEditor.PlayerSettings.iOS.locationUsageDescription = "Details to use location";
#endif
        }

        [Serializable]
        public class DebugStartLocation
        {
            #pragma warning disable 0649
            [SerializeField] private string name;
            [SerializeField] private GPSLatLong latLong;
            #pragma warning restore 0649
            
            public string Name => this.name;

            public GPSLatLong LatLong => this.latLong;
        }
    }
}

#endif
