#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="Releases.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
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
#pragma warning disable 0649
        [SerializeField] private List<Release> releases = new List<Release> { new Release { AppVersion = "0.1.0" } };
#pragma warning restore 0649

        public Release CurrentRelease => this.releases.LastOrDefault();

        public List<Release> AllReleases => this.releases;

        [EditorEvents.OnEnterPlayMode]
        [EditorEvents.OnPreprocessBuild]
        public static void SaveCurrentReleaseToResources()
        {
            string path = $"Assets/Resources/{ReleaseLocator.ReleasesResourcesName}.json";
            File.WriteAllText(path, JsonUtil.Serialize(LostLibrary.Releases.CurrentRelease));
            UnityEditor.AssetDatabase.ImportAsset(path);
        }
    }
}
