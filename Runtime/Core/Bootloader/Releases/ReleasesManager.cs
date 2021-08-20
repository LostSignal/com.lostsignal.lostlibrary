//-----------------------------------------------------------------------
// <copyright file="ReleasesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections;
    using UnityEngine;

    ////
    //// NEED TO MAKE A RELEASES APP CONFIG
    //// Will have the URL
    //// WIll have the blob storage upload info
    ////
    public sealed class ReleasesManager : Manager<ReleasesManager>
    {
        public enum StorageLocation
        {
            Resources,
            PlayFab,
        }

        #pragma warning disable 0649
        [SerializeField] private StorageLocation storageLocation;
        #pragma warning restore 0649

        public Release CurrentRelease { get; private set; }

        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                if (this.storageLocation == StorageLocation.Resources)
                {
                    this.CurrentRelease = ReleaseLocator.GetCurrentReleaseFromResources();
                }
                else if (this.storageLocation == StorageLocation.PlayFab)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new Exception($"Unknown StorageLocation {this.storageLocation} Found!");
                }

                this.SetInstance(this);

                yield break;
            }
        }

        public Coroutine ShowForceUpdateDialog()
        {
            return this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                // TODO [bgish]: Implement
                yield break;
            }
        }
    }
}

#endif
