//-----------------------------------------------------------------------
// <copyright file="TiinooManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public sealed class TiinooManager : Manager<TiinooManager>
    {
        #if USING_TIINOO
        #pragma warning disable 0649, 0414
        [SerializeField] private int canvasSortOrder = 30000;
        [SerializeField] private int uiLayer;
        [SerializeField] private Settings devBuildSettings = Settings.DefaultDevSettings;
        [SerializeField] private Settings relaseBuildSettings = Settings.DefaultLiveSettings;
        #pragma warning restore 0649, 0414
        #endif

        #if !USING_TIINOO

        [ExposeInEditor("Download Tiinoo")]
        private void OpenUrl()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/device-console-44935");
        }

        [ExposeInEditor("Add USING_TIINOO Define")]
        private void AddUsingTiinooDefine()
        {
            ProjectDefinesHelper.AddDefineToProject("USING_TIINOO");
        }

        #endif

        public override void Initialize()
        {
            #if USING_TIINOO

            var settings = Debug.isDebugBuild ? this.devBuildSettings : this.relaseBuildSettings;

            if (settings.Enabled)
            {
                Tiinoo.DeviceConsole.DCSettings.Instance.canvasSortOrder = this.canvasSortOrder;
                Tiinoo.DeviceConsole.DCSettings.Instance.uiLayer = this.uiLayer;
                Tiinoo.DeviceConsole.DCSettings.Instance.openWithGesture = settings.OpenWithGesture;
                Tiinoo.DeviceConsole.DCSettings.Instance.openWithKey = settings.OpenWithKey;
                Tiinoo.DeviceConsole.DCSettings.Instance.exceptionNotification = settings.ExceptionNotification;
                Tiinoo.DeviceConsole.DCLoader.Load();
            }

            #else

            Debug.LogError("Trying to use Tiinoo 3rd party plugin without the USING_TIINOO define");

            #endif

            this.SetInstance(this);
        }

        private void OnDestroy()
        {
            #if USING_TIINOO
            Tiinoo.DeviceConsole.DCLoader.Unload();
            #endif
        }

#if USING_TIINOO
        [System.Serializable]
        public class Settings
        {
            #pragma warning disable 0649
            [SerializeField] private bool enabled;
            [SerializeField] private Tiinoo.DeviceConsole.DCSettings.Gesture gesture;
            [SerializeField] private KeyCode keyCode;
            [SerializeField] private bool exceptionNotification;
            #pragma warning restore 0649

            public static Settings DefaultDevSettings => new Settings
            {
                enabled = true,
                gesture = Tiinoo.DeviceConsole.DCSettings.Gesture.SWIPE_DOWN_WITH_3_FINGERS,
                keyCode = KeyCode.F1,
                exceptionNotification = true,
            };

            public static Settings DefaultLiveSettings => new Settings
            {
                enabled = true,
                gesture = Tiinoo.DeviceConsole.DCSettings.Gesture.SWIPE_DOWN_WITH_3_FINGERS,
                keyCode = KeyCode.F1,
                exceptionNotification = false,
            };

            public bool Enabled => this.enabled;

            public Tiinoo.DeviceConsole.DCSettings.Gesture OpenWithGesture => this.gesture;

            public KeyCode OpenWithKey => this.keyCode;

            public bool ExceptionNotification => this.exceptionNotification;
        }
        #endif
    }
}

#endif
