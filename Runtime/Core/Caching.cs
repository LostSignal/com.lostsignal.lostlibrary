//-----------------------------------------------------------------------
// <copyright file="Caching.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using Lost.Networking;
    using UnityEngine;

    public static class Caching
    {
        // General
        public static readonly byte[] ByteBuffer = new byte[1024 * 1024]; // 1 MB

        // Netwokring
        public static readonly List<NetworkIdentity> NetworkIdentities = new List<NetworkIdentity>(50);

        // Physics
        public static readonly ContactPoint[] ContactPointsCache = new ContactPoint[50];
    }
}

#endif
