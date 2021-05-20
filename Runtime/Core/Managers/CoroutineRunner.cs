//-----------------------------------------------------------------------
// <copyright file="CoroutineRunner.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public class CoroutineRunner : Manager<CoroutineRunner>
    {
        public static Coroutine Start(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        public override void Initialize()
        {
            this.SetInstance(this);
        }
    }
}
