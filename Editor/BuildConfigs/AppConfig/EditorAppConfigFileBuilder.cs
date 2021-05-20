//-----------------------------------------------------------------------
// <copyright file="EditorAppConfigFileBuidler.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.VersionControl;

    public static class EditorAppConfigFileBuidler
    {
        [MenuItem("Tools/Lost/Work In Progress/Generate AppConfigs C# File", false)]
        public static void GenerateAppConfigsFile()
        {
            StringBuilder file = new StringBuilder();
            file.AppendLine("// <auto-generated/>              ".TrimEnd());
            file.AppendLine("#pragma warning disable           ".TrimEnd());
            file.AppendLine("                                  ".TrimEnd());
            file.AppendLine("using Lost.AppConfig;             ".TrimEnd());
            file.AppendLine("using UnityEditor;                ".TrimEnd());
            file.AppendLine("                                  ".TrimEnd());
            file.AppendLine("public static class AppConfigs    ".TrimEnd());
            file.AppendLine("{                                 ".TrimEnd());
            file.Append(GetConstants());
            file.Append(GetMethods());
            file.AppendLine("}");

            // Writing out to disk and refershing
            string path = EditorAppConfig.AppConfigScriptPath;

            if (File.Exists(path) && Provider.isActive)
            {
                var asset = AssetDatabase.LoadAllAssetsAtPath(path);
                Provider.Checkout(asset, CheckoutMode.Asset).Wait();
            }

            File.WriteAllText(path, file.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }

        private static string GetConstants()
        {
            StringBuilder constants = new StringBuilder();

            foreach (var appConfig in LostLibrary.AppConfigs.AppConfigs)
            {
                // If we have more than one config, then skip the root config
                if (LostLibrary.AppConfigs.AppConfigs.Count > 1 && appConfig == LostLibrary.AppConfigs.RootAppConfig)
                {
                    continue;
                }

                StringBuilder constantsBuilder = new StringBuilder();
                constantsBuilder.AppendLine("    private const string Config{config_name}Path = \"{menu_item_name}\";");
                constantsBuilder.AppendLine("    private const string Config{config_name}Guid = \"{config_guid}\";");

                string constantsString = constantsBuilder.ToString()
                    .Replace("{config_name}", appConfig.SafeName)
                    .Replace("{config_guid}", appConfig.Id)
                    .Replace("{menu_item_name}", GetMenuItemName(appConfig));

                constants.AppendLine(constantsString);
            }

            return constants.ToString();
        }

        private static string GetMethods()
        {
            List<string> methods = new List<string>();

            foreach (var appConfig in LostLibrary.AppConfigs.AppConfigs)
            {
                // If we have more than one config, then skip the root config
                if (LostLibrary.AppConfigs.AppConfigs.Count > 1 && appConfig == LostLibrary.AppConfigs.RootAppConfig)
                {
                    continue;
                }

                StringBuilder methodBuilder = new StringBuilder();
                methodBuilder.AppendLine("    [MenuItem(Config{config_name}Path, false, 0)]                                                           ".TrimEnd());
                methodBuilder.AppendLine("    public static void Set{config_name}Config()                                                             ".TrimEnd());
                methodBuilder.AppendLine("    {                                                                                                       ".TrimEnd());
                methodBuilder.AppendLine("        EditorAppConfig.SetActiveConfig(Config{config_name}Guid);                                           ".TrimEnd());
                methodBuilder.AppendLine("    }                                                                                                       ".TrimEnd());
                methodBuilder.AppendLine("                                                                                                            ".TrimEnd());
                methodBuilder.AppendLine("    [MenuItem(Config{config_name}Path, true, 0)]                                                            ".TrimEnd());
                methodBuilder.AppendLine("    private static bool Set{config_name}ConfigValidate()                                                    ".TrimEnd());
                methodBuilder.AppendLine("    {                                                                                                       ".TrimEnd());
                methodBuilder.AppendLine("        Menu.SetChecked(Config{config_name}Path, EditorAppConfig.IsActiveConfig(Config{config_name}Guid));  ".TrimEnd());
                methodBuilder.AppendLine("        return true;                                                                                        ".TrimEnd());
                methodBuilder.AppendLine("    }                                                                                                       ".TrimEnd());

                methods.Add(methodBuilder.ToString().Replace("{config_name}", appConfig.SafeName));
            }

            // Constructing the final methods string
            StringBuilder methodsBuilder = new StringBuilder();

            for (int i = 0; i < methods.Count; i++)
            {
                if (i != methods.Count - 1)
                {
                    methodsBuilder.AppendLine(methods[i]);
                }
                else
                {
                    methodsBuilder.Append(methods[i]);
                }
            }

            return methodsBuilder.ToString();
        }

        private static string GetSafeAppConfigName(AppConfig appConfig)
        {
            return appConfig.Parent != null ?
                GetSafeAppConfigName(appConfig.Parent) + "/" + appConfig.SafeName :
                appConfig.SafeName;
        }

        private static string GetMenuItemName(AppConfig appConfig)
        {
            string menuItemName = "Tools/Lost/Configs/" + GetSafeAppConfigName(appConfig);

            // Removing the root config from the menu item path if we have more than one configs
            if (LostLibrary.AppConfigs.AppConfigs.Count > 1)
            {
                menuItemName = menuItemName.Replace(LostLibrary.AppConfigs.RootAppConfig.SafeName + "/", string.Empty);
            }

            return menuItemName;
        }
    }
}
