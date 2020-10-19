//-----------------------------------------------------------------------
// <copyright file="AddressablesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public class AddressablesManager : Manager<AddressablesManager>
    {
        #pragma warning disable 0649
        [Header("Dependencies")]
        [SerializeField] private ReleasesManager releasesManager;
        #pragma warning restore 0649

        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                yield return this.WaitForDependencies(this.releasesManager);

                //// TODO [bgish]: Set the addressables location
                //// this.releasesManager.CurrentRelease.AddressablesLocation;

                yield return UnityEngine.AddressableAssets.Addressables.InitializeAsync();

                this.SetInstance(this);
            }
        }
    }
}
