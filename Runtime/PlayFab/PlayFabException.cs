#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="PlayFabException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using System;
    using global::PlayFab;

    public class PlayFabException : Exception
    {
        public PlayFabException(PlayFabError error)
        {
            this.Error = error;
        }

        public PlayFabError Error { get; private set; }
    }
}
