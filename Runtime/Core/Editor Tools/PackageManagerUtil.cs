#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="PackageManagerUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

namespace Lost
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEditor.PackageManager.Requests;
    using UnityEngine;

    public static class PackageManagerUtil
    {
        private static List<string> packageIdsToAdd = new List<string>();
        private static AddRequest addRequest = null;
        private static bool isProcessing = false;

        public static void Add(string packageId)
        {
            packageIdsToAdd.Add(packageId);

            if (isProcessing == false)
            {
                isProcessing = true;
                EditorApplication.update += ProcessList;
            }
        }

        private static void ProcessList()
        {
            if (packageIdsToAdd.Count > 0 && addRequest == null)
            {
                var packageId = packageIdsToAdd[0];
                packageIdsToAdd.RemoveAt(0);
                addRequest = Client.Add(packageId);
            }
            else if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"Package {addRequest.Result.packageId} Installed Successfully");
                }
                else if (addRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError($"Failed To Install Package {addRequest.Result.packageId}: {addRequest.Error.message}");
                }

                addRequest = null;
            }

            if (packageIdsToAdd.Count == 0 && addRequest == null)
            {
                EditorApplication.update -= ProcessList;
                isProcessing = false;
            }
        }
    }
}

#endif
