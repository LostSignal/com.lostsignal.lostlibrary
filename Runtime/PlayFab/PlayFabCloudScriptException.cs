#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="PlayFabCloudScriptException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using System;

    public class PlayFabCloudScriptException : Exception
    {
        public PlayFabCloudScriptException(string error)
        {
            this.Error = error;
        }

        public string Error { get; private set; }
    }
}
