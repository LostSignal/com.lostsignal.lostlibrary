//-----------------------------------------------------------------------
// <copyright file="PackageHelperEditorWindow.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//// https://docs.unity3d.com/Manual/upm-api.html

namespace Lost
{
    using System.Collections.Generic;
    using UnityEditor;

    public class PackageHelperEditorWindow : EditorWindow
    {
        private static Dictionary<string, Dictionary<string, bool>> Packages = new Dictionary<string, Dictionary<string, bool>>();

        static PackageHelperEditorWindow()
        {
            var urp = new Dictionary<string, bool>();
            urp.Add("com.unity.render-pipelines.universal", false);

            var mobileAR = new Dictionary<string, bool>();
            mobileAR.Add("com.unity.xr.arfoundation", false);
            mobileAR.Add("com.unity.xr.arcore", false);
            mobileAR.Add("com.unity.xr.arkit", false);
            mobileAR.Add("com.unity.xr.arkit-face-tracking", false);

            var xr = new Dictionary<string, bool>();
            xr.Add("com.unity.xr.oculus", false);
            xr.Add("com.unity.xr.windowsmr", false);
            xr.Add("com.unity.xr.magicleap", false);

            var optional = new Dictionary<string, bool>();
            optional.Add("com.unity.timeline", false);
            optional.Add("com.unity.cinemachine", false);
            optional.Add("com.unity.cloud.userreporting", false);
            optional.Add("com.unity.ads", false);
            optional.Add("com.unity.analytics", false);
            optional.Add("com.unity.purchasing", false);
            optional.Add("com.unity.purchasing.udp", false);

            var optionalEditor = new Dictionary<string, bool>();
            optionalEditor.Add("com.unity.editorcoroutines", false);
            optionalEditor.Add("com.unity.build-report-inspector", false);
            optionalEditor.Add("com.unity.assetbundlebrowser", false);
            optionalEditor.Add("com.unity.device-simulator", false);
            optionalEditor.Add("com.unity.memoryprofiler", false);
            optionalEditor.Add("com.unity.mobile.android-logcat", false);
            optionalEditor.Add("com.unity.mobile.notifications", false);
            optionalEditor.Add("com.unity.performance.profile-analyzer", false);
            optionalEditor.Add("com.unity.formats.fbx", false);
            optionalEditor.Add("com.unity.probuilder", false);
            optionalEditor.Add("com.unity.progrids", false);
            optionalEditor.Add("com.unity.polybrush", false);
            optionalEditor.Add("com.unity.quicksearch", false);
            optionalEditor.Add("com.unity.ui.builder", false);

            Packages.Add("URP", urp);
            Packages.Add("Mobile AR", mobileAR);
            Packages.Add("XR", xr);
            Packages.Add("Optional", optional);
            Packages.Add("Optional Editor", optionalEditor);
        }

        [MenuItem("Tools/Lost/Tools/Packages Helper")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<PackageHelperEditorWindow>(false, "Packages Helper");
        }

        private void OnGUI()
        {
        }
    }
}
