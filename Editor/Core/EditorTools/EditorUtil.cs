//-----------------------------------------------------------------------
// <copyright file="EditorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;

    public static class EditorUtil
    {
        public static T GetAssetByGuid<T>(string guid) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
