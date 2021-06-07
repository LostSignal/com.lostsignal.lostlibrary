//-----------------------------------------------------------------------
// <copyright file="GameServerProjectGenerator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.BuildConfig;
    using Lost.PlayFab;

    public class GameServerProjectGenerator : CSharpProjectGenerator
    {
        protected override void WriteFile(string filePath, string contents)
        {
            var playfab = EditorBuildConfigs.ActiveBuildConfig.GetSettings<PlayFabSettings>();

            contents = contents
                .Replace("__PLAYFAB_TITLE_ID__", playfab.TitleId)
                .Replace("__PLAYFAB_SECRET_KEY__", playfab.SecretKey);

            base.WriteFile(filePath, contents);
        }
    }
}
