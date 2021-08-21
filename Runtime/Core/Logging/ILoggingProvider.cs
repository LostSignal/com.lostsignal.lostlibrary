#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ILoggingProvider.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public interface ILoggingProvider
    {
        void Log(string condition, string stackTrace, LogType type);
    }
}

#endif
