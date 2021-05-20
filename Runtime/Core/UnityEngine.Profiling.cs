//-----------------------------------------------------------------------
// <copyright file="UnityEngine.Profiling.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable

#if !UNITY

namespace UnityEngine.Profiling
{
    public static class Profiler
    {
        public static void BeginSample(string str, object obj = null)
        {
        }

        public static void EndSample()
        {
        }
    }
}

#endif
