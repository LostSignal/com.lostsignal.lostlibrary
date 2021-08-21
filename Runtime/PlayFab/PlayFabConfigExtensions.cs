﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="PlayFabRuntimBuildConfigExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.PlayFab
{
    using Lost.BuildConfig;

    public static class PlayFabConfigExtensions
    {
        public static readonly string TitleId = "PlayFab.TitleId";

        public static string GetTitleId(this RuntimeBuildConfig runtimeConfig)
        {
            return runtimeConfig.GetString(TitleId);
        }
    }
}

#endif
