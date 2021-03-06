//-----------------------------------------------------------------------
// <copyright file="AddressablesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections;

    public sealed class AddressablesManager : Manager<AddressablesManager>
    {
        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                yield return ReleasesManager.WaitForInitialization();

                //// TODO [bgish]: Set the addressables location
                //// this.releasesManager.CurrentRelease.AddressablesLocation;

                yield return UnityEngine.AddressableAssets.Addressables.InitializeAsync();

                this.SetInstance(this);
            }
        }
    }
}

#endif
