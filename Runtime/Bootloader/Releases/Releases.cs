//-----------------------------------------------------------------------
// <copyright file="Releases.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    //// var url = RuntimeAppConfig.Instance.GetString(ReleasesUrlKey).AppendIfDoesntExist("/");
    //// var machineName = RuntimeAppConfig.Instance.GetString(ReleasesMachineNameKey);
    ////
    //// var fullUrl = Platform.IsUnityCloudBuild ?
    ////     $"{url}{ReleasesJsonFileName}" :
    ////     $"{url}{machineName}/{ReleasesJsonFileName}";
    ////
    //// TODO [bgish]: Download Releases.json using the fullUrl
    //// TODO [bgish]: Calculate and set this.CurrentRelease based on RuntimeAppConfig.Instance.Version/BuildNumber/CommitId
    //// TODO [bgish]: Check for any force updates
    ////
    //// JsonUtil.Deserialize<Release>(RuntimeBuildConfig.Instance.GetString(ReleasesCurrentRelease));
    ////

    public class Releases : ScriptableObject
    {
        public const string ReleasesResourcesName = "release.json";

        public enum StorageLocation
        {
            Resources,
        }

#pragma warning disable 0649
        [SerializeField] private StorageLocation location;
        [SerializeField] private List<Release> releases = new List<Release> { new Release { AppVersion = "0.1.0" } };
#pragma warning restore 0649

        public Release CurrentRelease => this.releases.LastOrDefault();
        
        public List<Release> AllReleases => this.releases;

        public static Release GetCurrentRelease()
        {
            #if UNITY_EDITOR
            if (Application.isEditor)
            {
                Releases releases = UnityEditor.AssetDatabase.LoadAssetAtPath<Releases>("Assets/Editor/com.lostsignal.lostlibrary/Releases.asset");
                return releases.releases.LastOrDefault();
            }
            #endif

            // Will need to load this from resources, or eventually PlayFab
            throw new NotImplementedException();
        }

        #if UNITY_EDITOR
        // [EditorEvents.OnBuild, EditorEvents.OnEnterPlayMode]
        private static void SaveReleases()
        {
            Releases releases = UnityEditor.AssetDatabase.LoadAssetAtPath<Releases>("Assets/Editor/com.lostsignal.lostlibrary/Releases");
            Release currentRelease = releases.releases.LastOrDefault();
            System.IO.File.WriteAllText($"Assets/Resources/{ReleasesResourcesName}", JsonUtil.Serialize(currentRelease));
            GameObject.DestroyImmediate(releases);
        }
        #endif
    }
}

#endif
