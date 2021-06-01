//-----------------------------------------------------------------------
// <copyright file="ReleasesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

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
                this.CurrentRelease = Releases.GetCurrentRelease();
                this.SetInstance(this);
                yield break;
            }
        }
    }
}

#endif
