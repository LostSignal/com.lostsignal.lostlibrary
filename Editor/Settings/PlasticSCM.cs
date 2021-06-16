//-----------------------------------------------------------------------
// <copyright file="PlasticSCM.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public static class PlasticSCM
    {
        public static string GetClientConfigPath()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\plastic4\\client.conf";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.plastic4/client.conf";
            }

            return null;
        }

        public static void UpdateClientConfigSettings(string clientConfigPath, bool updateSameItemDifferentCaseError, bool updateYamlMergeTool)
        {
            if (string.IsNullOrEmpty(clientConfigPath) || File.Exists(clientConfigPath) == false)
            {
                Debug.LogError($"Lost.PlastSCM couldn't find client config {clientConfigPath}");
                return;
            }

            var newClientConfigBuilder = new StringBuilder();

            foreach (var line in File.ReadAllLines(clientConfigPath))
            {
                string newLine = line;

                if (updateSameItemDifferentCaseError && line.Contains("<SameItemDifferentCaseError>") && line.Contains("no"))
                {
                    newLine = line.Replace("no", "yes");
                }
                else if (updateYamlMergeTool && line.Contains("UnityYAMLMerge.exe"))
                {
                    int stringStartIndex = newLine.IndexOf("<string>\"");
                    int folderStartIndex = stringStartIndex + "<string>\"".Length;
                    int folderEndIndex = newLine.IndexOf("UnityYAMLMerge.exe");

                    string yamlMergeFolder = newLine.Substring(folderStartIndex, folderEndIndex - folderStartIndex - 1);

                    if (Directory.Exists(yamlMergeFolder) == false)
                    {
                        var latestUnityVersion = Directory.GetDirectories("C:\\Program Files\\Unity\\Hub\\Editor").LastOrDefault();

                        if (string.IsNullOrEmpty(latestUnityVersion) == false)
                        {
                            newLine = newLine.Remove(folderStartIndex, folderEndIndex - folderStartIndex);
                            newLine = newLine.Insert(folderStartIndex, latestUnityVersion + "\\Editor\\Data\\Tools\\");
                        }
                    }
                }

                if (newLine == "</ClientConfigData>")
                {
                    newClientConfigBuilder.Append(newLine);
                }
                else
                {
                    newClientConfigBuilder.AppendLine(newLine);
                }
            }

            var newClientConfig = newClientConfigBuilder.ToString();

            if (File.ReadAllText(clientConfigPath) != newClientConfig)
            {
                File.WriteAllText(clientConfigPath, newClientConfig);
            }
        }
    }
}
