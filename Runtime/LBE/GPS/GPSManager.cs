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
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.InputSystem;

    ////
    //// TODO [bgish]: Maybe don't just send the current Lat/Long but take a rolling average to smooth out the data?
    ////
    //// https://stackoverflow.com/questions/53046670/how-to-get-values-from-methods-written-in-ios-plugin-in-unity
    //// https://medium.com/@nosuchstudio/how-to-access-gps-location-in-unity-521f1371a7e3
    ////
    public sealed class GPSManager :
        Manager<GPSManager>
    {
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

#pragma warning disable 0649
        [SerializeField] private GPSAccuracy accuracy;
        [SerializeField] private bool waitForUnityRemote;
        [SerializeField] private bool startGpsServiceOnBoot = true;
        [SerializeField] private bool disableGpsWhenLostFocus = true;
        [SerializeField] private float updateFrequencyInSeconds = 1.0f;
        [SerializeField] private bool appMustUseGps = false;

        [Header("Debug")]
        [SerializeField] private bool printDebugOutput;
        [SerializeField] private bool allowWasdInEditor = true;
        [SerializeField] private bool smoothMovementInEdtior = true;
        [SerializeField] private double latLongSpeed = 0.001f;
        [SerializeField] private List<DebugStartLocation> debugStartLocations;
#pragma warning restore 0649

        [EditorEvents.OnPostGenerateGradleAndroidProject]
        public static void OnPostGenerateGradleAndroidProject(string dir)
        {
            var manifestPath = System.IO.Path.Combine(dir, "src/main/AndroidManifest.xml").Replace(@"\", "/");
            var manifestXml = System.IO.File.ReadAllText(manifestPath);
            manifestXml = InsertPermission(manifestXml, "ACCESS_COARSE_LOCATION");
            manifestXml = InsertPermission(manifestXml, "ACCESS_FINE_LOCATION");
            System.IO.File.WriteAllText(manifestPath, manifestXml);

            string InsertPermission(string xml, string permission)
            {
                if (xml.Contains(permission) == false)
                {
                    int index = xml.IndexOf("</manifest>");
                    return xml.Insert(index, $"  <uses-permission android:name=\"android.permission.{permission}\" />{Environment.NewLine}");
                }

                return xml;
            }
        }

        private bool hasReceivedGpsData;
        private GPSLatLong currentRawLatLong;

        private GPSServiceState serviceState;
        private Coroutine serviceCoroutine;

        private bool hasEditorLatLongBeenSet;
        private GPSLatLong editorLatLong;

        public bool HasReceivedGpsData => this.hasReceivedGpsData;

        public Action<GPSLatLong> OnGPSReceived;

        public bool IsRunning => this.serviceState == GPSServiceState.Running;

        public bool IsStartingUp => this.serviceState == GPSServiceState.StartingUp;

        public bool IsStopped => this.serviceState == GPSServiceState.Stopped;

        public GPSLatLong CurrentRawLatLong
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.currentRawLatLong;
            }

            private set
            {
                this.currentRawLatLong = value;
                this.hasReceivedGpsData = true;

                try
                {
                    this.OnGPSReceived?.Invoke(this.currentRawLatLong);
                }
                catch (Exception ex)
                {
                    Debug.LogError("GPSManager caught exception when invoking OnGPSReceived.");
                    Debug.LogException(ex);
                }
            }
        }

        public override void Initialize()
        {
            this.EnsurePlayerSettingsAreCorrect();
            this.SetInstance(this);

            if (this.startGpsServiceOnBoot)
            {
                this.StartGpsService();
            }
        }

        public void StartGpsService()
        {
            this.StopGpsService();

            if (Application.isEditor && this.waitForUnityRemote == false)
            {
                this.serviceCoroutine = this.StartCoroutine(StartGpsServiceEditorCoroutine());
            }
            else if (Application.isEditor || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                this.serviceCoroutine = this.StartCoroutine(StartGpsServiceCoroutine());
            }
            else
            {
                Debug.LogError($"GPSManager Encountered Unknown Platform {Application.platform}");
            }

            IEnumerator StartGpsServiceEditorCoroutine()
            {
                if (this.printDebugOutput)
                {
                    Debug.Log("GPSManager.StartGpsServiceEditorCoroutine");
                }

                this.serviceState = GPSServiceState.StartingUp;

                if (this.hasEditorLatLongBeenSet == false)
                {
                    var debugStartLocationsCount = this.debugStartLocations?.Count;

                    if (debugStartLocationsCount == null || debugStartLocationsCount == 0)
                    {
                        Debug.LogError("GPSManager has no debug start locations to work with.");
                        yield break;
                    }

                    GPSLatLong latLong = this.debugStartLocations[0].LatLong;

                    if (debugStartLocationsCount > 1)
                    {
                        // TODO [bgish]: Let user pick a location
                    }

                    this.editorLatLong = new GPSLatLong
                    {
                        Latitude = latLong.Latitude,
                        Longitude = latLong.Longitude,
                    };

                    this.hasEditorLatLongBeenSet = true;
                }

                this.serviceState = GPSServiceState.Running;

                while (true)
                {
                    this.CurrentRawLatLong = this.editorLatLong;
                    yield return WaitForUtil.Seconds(this.smoothMovementInEdtior ? 0.02f : this.updateFrequencyInSeconds);
                }
            }

            IEnumerator StartGpsServiceCoroutine()
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

                // TODO [bgish]: Need to not do this look if we're just running in editor and this.waitForUnityRemote == false
                while (true)
                {
                    bool isGpsEnabled = GPSUtil.IsGpsEnabledByUser();

                    if (isGpsEnabled == false)
                    {
                        GPSUtil.AskForPermissionToUseGPS();
                        yield return WaitForUtil.Seconds(1.0f);
                        isGpsEnabled = GPSUtil.IsGpsEnabledByUser();
                    }

                    if (isGpsEnabled)
                    {
                        break;
                    }
                    else if (this.appMustUseGps == false)
                    {
                        Debug.LogError("Unable to start GPS Manage.  The user doesn't have permissions.");
                        this.StopGpsService();
                        break;
                    }
                    else
                    {
                        var gpsRequired = LostMessages.GpsIsRequiredRetryOrQuitApp();

                        yield return gpsRequired;

                        if (gpsRequired.Value == YesNoResult.No)
                        {
                            Platform.QuitApplication();
                        }
                    }
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
                        this.CurrentRawLatLong = GPSUtil.GetGPSLatLong();
                        yield return WaitForUtil.Seconds(this.updateFrequencyInSeconds);
                    }
                }
            }
        }

        public void StopGpsService()
        {
            if (this.serviceCoroutine != null)
            {
                this.StopCoroutine(this.serviceCoroutine);
                this.serviceCoroutine = null;
            }

            if (!Application.isEditor || this.waitForUnityRemote)
            {
                UnityEngine.Input.location.Stop();
            }

            this.serviceState = GPSServiceState.Stopped;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (Application.isEditor || this.disableGpsWhenLostFocus == false)
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

#if UNITY_EDITOR
        private void Update()
        {
            if (this.allowWasdInEditor == false || this.waitForUnityRemote)
            {
                return;
            }

            Vector2d movement = new Vector2d();

#if USING_UNITY_INPUT_SYSTEM

            if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed)
            {
                movement += new Vector2d(1.0, 0.0);
            }

            if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed)
            {
                movement += new Vector2d(-1.0, 0.0);
            }

            if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed)
            {
                movement += new Vector2d(0.0, -1.0);
            }

            if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed)
            {
                movement += new Vector2d(0.0, 1.0);
            }

#else

            if (UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                movement += new Vector2d(1.0, 0.0);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.S))
            {
                movement += new Vector2d(-1.0, 0.0);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                movement += new Vector2d(0.0, -1.0);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.D))
            {
                movement += new Vector2d(0.0, 1.0);
            }

#endif

            if (movement != Vector2d.zero)
            {
                var from = Vector3.forward;
                var to = Camera.main.transform.forward.SetY(0);
                var cameraAngle = Vector3.Angle(from, to);
                var sign = Vector3.Dot(Vector3.Cross(to, from), Vector3.up) > 0 ? -1 : 1;

                movement *= (this.latLongSpeed * Time.deltaTime);
                movement = movement.Rotate(cameraAngle * sign);

                this.editorLatLong.Latitude += movement.x;
                this.editorLatLong.Longitude += movement.y;
            }
        }

#endif

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
