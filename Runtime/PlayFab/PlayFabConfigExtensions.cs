//-----------------------------------------------------------------------
// <copyright file="PlayFabRuntimBuildConfigExtensions.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
