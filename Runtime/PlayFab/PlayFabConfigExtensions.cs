//-----------------------------------------------------------------------
// <copyright file="PlayFabRuntimBuildConfigExtensions.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using Lost.AppConfig;

    public static class PlayFabConfigExtensions
    {
        public static readonly string TitleId = "PlayFab.TitleId";

        public static string GetTitleId(this RuntimeAppConfig runtimeConfig)
        {
            return runtimeConfig.GetString(TitleId);
        }
    }
}
