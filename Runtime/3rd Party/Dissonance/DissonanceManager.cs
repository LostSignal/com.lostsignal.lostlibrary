#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="DissonanceManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.DissonanceIntegration
{
    using Lost.PlayFab;
    using UnityEngine;

    public sealed class DissonanceManager : Manager<DissonanceManager>
    {
#pragma warning disable 0649
        [SerializeField] private GameObject dissonanceCommsPrefab;
#pragma warning restore 0649

#if USING_DISSONANCE
        public Dissonance.DissonanceComms DissonanceComms { get; private set; }
#endif

        public void RequestMicrophonePermissions()
        {
#if PLATFORM_ANDROID
            var microphonePermission = UnityEngine.Android.Permission.Microphone;

            if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(microphonePermission) == false)
            {
                UnityEngine.Android.Permission.RequestUserPermission(microphonePermission);
            }
#endif
        }

        public override void Initialize()
        {
            this.RequestMicrophonePermissions();
            this.StartCoroutine(Coroutine());

            System.Collections.IEnumerator Coroutine()
            {
                yield return ReleasesManager.WaitForInitialization();
                yield return PlayFabManager.WaitForInitialization();

                var settings = ReleasesManager.Instance.CurrentRelease.DissonanceManagerSettings;

#if !USING_DISSONANCE

                if (this.enabled)
                {
                    Debug.LogError("DissonanceManager is enabled, but USING_DISSONANCE define is not set.  Make sure Dissonance plugin is " +
                        "installed and the define is set, otherwise Dissonance will not work.", this);
                }

#else
                if (this.dissonanceCommsPrefab == null)
                {
                    Debug.LogError("DissonanceManager: Unable to locate the DissonanceComms object. Dissonance will not work.", this);
                }
                else
                {
                    var dissonanceCommsObject = GameObject.Instantiate(this.dissonanceCommsPrefab, this.transform);

                    this.DissonanceComms = dissonanceCommsObject.GetComponent<Dissonance.DissonanceComms>();

                    if (this.DissonanceComms == null)
                    {
                        Debug.LogError("DissonanceManager: DissonanceComms Prefab does not have the DissonanceComms Component, Dissonance will not work.", this);
                    }
                    else
                    {
                        this.DissonanceComms.LocalPlayerName = PlayFabManager.Instance.User.PlayFabId;
                        this.DissonanceComms.gameObject.name = "Dissonance Comms";
                        this.DissonanceComms.gameObject.SetActive(true);
                    }
                }
#endif

                this.SetInstance(this);
            }
        }

        [System.Serializable]
        public class Settings
        {
#pragma warning disable 0649
            [SerializeField] private bool isEnabled;
#pragma warning restore 0649

            public bool IsEnabled
            {
                get => this.isEnabled;
                set => this.isEnabled = value;
            }
        }
    }
}

#endif
