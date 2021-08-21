#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="DontDestroyOnLoad.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
    }
}

#endif
