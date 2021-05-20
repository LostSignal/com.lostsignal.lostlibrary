//-----------------------------------------------------------------------
// <copyright file="ReleasesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using Lost.BuildConfig;

    //// NEED TO MAKE A RELEASES APP CONFIG
    //// Will have the URL
    //// WIll have the blob storage upload info
    ////
    public class ReleasesManager : Manager<ReleasesManager>
    {
        public const string ReleasesJsonFileName = "Releases.json";
        public const string ReleasesUrlKey = "Releases.URL";
        public const string ReleasesMachineNameKey = "Releases.MachineName";
        public const string ReleasesCurrentRelease = "Releases.CurrentRelease";

        public Release CurrentRelease { get; private set; }

        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
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

                this.CurrentRelease = JsonUtil.Deserialize<Release>(RuntimeBuildConfig.Instance.GetString(ReleasesCurrentRelease));

                this.SetInstance(this);

                yield break;
            }
        }
    }
}
