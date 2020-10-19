//-----------------------------------------------------------------------
// <copyright file="GameServerProjectGenerator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using Lost.PlayFab;

    public class GameServerProjectGenerator : CSharpProjectGenerator
    {
        protected override void WriteFile(string filePath, string contents)
        {
            var playfab = EditorAppConfig.ActiveAppConfig.GetSettings<PlayFabSettings>();

            contents = contents
                .Replace("__PLAYFAB_TITLE_ID__", playfab.TitleId)
                .Replace("__PLAYFAB_SECRET_KEY__", playfab.SecretKey);

            base.WriteFile(filePath, contents);
        }
    }
}
