//-----------------------------------------------------------------------
// <copyright file="ForcePancakeInEditorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public static class ForcePancakeInEditorUtil
    {
        private const string Key = "ForcePancakeInEditor";
        private const string ForcePancakeInEditorPath = "Tools/Force Pancake In Editor";

#if !UNITY_EDITOR
        public static bool ForcePancakeInEditor => false;
#else

        public static bool ForcePancakeInEditor
        {
            get => UnityEditor.EditorPrefs.GetBool($"{UnityEditor.PlayerSettings.applicationIdentifier}-{Key}", true);
            set => UnityEditor.EditorPrefs.SetBool($"{UnityEditor.PlayerSettings.applicationIdentifier}-{Key}", value);
        }

        [UnityEditor.MenuItem(ForcePancakeInEditorPath, false, -1)]
        public static void SetForcePancakeInEditor()
        {
            ForcePancakeInEditor = !ForcePancakeInEditor;
        }

        [UnityEditor.MenuItem(ForcePancakeInEditorPath, true, -1)]
        private static bool SetForcePancakeInEditorValidate()
        {
            UnityEditor.Menu.SetChecked(ForcePancakeInEditorPath, ForcePancakeInEditor);
            return true;
        }

#endif
    }
}
