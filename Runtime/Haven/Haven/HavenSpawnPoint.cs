#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="HavenSpawnPoint.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using System.Collections;
    using UnityEngine;

    [AddComponentMenu("Haven XR/HXR Spawn Point")]
    public class HavenSpawnPoint : MonoBehaviour
    {
        private void Awake()
        {
            Bootloader.OnManagersReady += this.Initialize;
        }

        private void Initialize()
        {
            CoroutineRunner.Instance.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return HavenRig.WaitForRig();
                var rig = HavenRig.GetRig();
                rig.transform.position = this.transform.position;
            }
        }
    }
}

#endif
