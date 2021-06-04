//-----------------------------------------------------------------------
// <copyright file="Releases.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    ////
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
        public const string ReleasesResourcesName = "release";

#pragma warning disable 0649
        [SerializeField] private List<Release> releases = new List<Release> { new Release { AppVersion = "0.1.0" } };
#pragma warning restore 0649

        public Release CurrentRelease => this.releases.LastOrDefault();
        
        public List<Release> AllReleases => this.releases;

        public static Release GetCurrentReleaseFromResources()
        {
            var jsonAsset = Resources.Load<TextAsset>(ReleasesResourcesName);
            return JsonUtil.Deserialize<Release>(jsonAsset.text);
        }

        [EditorEvents.OnEnterPlayMode]
        [EditorEvents.OnPreprocessBuild]
        public static void SaveCurrentReleaseToResources()
        {
            #if UNITY_EDITOR
            Releases releases = UnityEditor.AssetDatabase.LoadAssetAtPath<Releases>("Assets/Editor/com.lostsignal.lostlibrary/Releases.asset");
            Release currentRelease = releases.releases.LastOrDefault();
            System.IO.File.WriteAllText($"Assets/Resources/{ReleasesResourcesName}.json", JsonUtil.Serialize(currentRelease));
            GameObject.DestroyImmediate(releases);
            #endif
        }
    }
}

#endif
