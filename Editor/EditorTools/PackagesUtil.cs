//-----------------------------------------------------------------------
// <copyright file="PackagesUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public class PackageUtilConfig
    {
        public List<string> LocalSourceDirectories { get; set; }

        public List<Repository> Repositories { get; set; }

        public class Repository
        {
            public string PackageIdentifier { get; set; }
            public string GitHubUrl { get; set; }
        }
    }

    public static class PackagesUtil
    {
        private const string ManifestPath = "./Packages/manifest.json";
        private const string PackageDiretcoriesFile = ".packageutil";
        private const string PackagesPath = "Packages/";

        public enum Mode
        {
            None,
            GitHub,
            LocalFolder,
        }

        [MenuItem("Assets/Package Util/Point To Local Source", false)]
        public static void PointToLocal()
        {
            SwitchRepositoryTo(Mode.LocalFolder);
        }

        [MenuItem("Assets/Package Util/Point To Local Source", true)]
        public static bool PointToLocalValidate()
        {
            GetRepository(out PackageUtilConfig.Repository repository, out string packageLocalPath, out Mode currentMode);
            return repository != null && currentMode != Mode.LocalFolder;
        }

        [MenuItem("Assets/Package Util/Point To GitHub", false)]
        public static void PointToGitHub()
        {
            SwitchRepositoryTo(Mode.GitHub);
        }

        [MenuItem("Assets/Package Util/Point To GitHub", true)]
        public static bool PointToGitHubValidate()
        {
            GetRepository(out PackageUtilConfig.Repository repository, out string packageLocalPath, out Mode currentMode);
            return repository != null && currentMode != Mode.GitHub;
        }

        private static void GetRepository(out PackageUtilConfig.Repository repository, out string packageLocalPath, out Mode currentMode)
        {
            repository = null;
            packageLocalPath = null;
            currentMode = Mode.None;

            PackageUtilConfig packageUtilConfig;

            try
            {
                packageUtilConfig = JsonUtil.Deserialize<PackageUtilConfig>(File.ReadAllText(PackageDiretcoriesFile));
            }
            catch (Exception ex)
            {
                Debug.LogError("Unable to read .packageutil file.");
                Debug.LogException(ex);
                return;
            }

            string packageName = GetPackageName();

            if (string.IsNullOrEmpty(packageName))
            {
                Debug.LogError("Invalid Package Folder Selected.");
                return;
            }

            foreach (var repo in packageUtilConfig.Repositories)
            {
                if (repo.PackageIdentifier == packageName)
                {
                    repository = repo;
                }
            }

            if (repository == null)
            {
                Debug.LogError($"Unable to find Package Identifier {packageName} in \".packageutil\" config file.");
                return;
            }

            packageLocalPath = GetPackageLocalPath(repository.PackageIdentifier, packageUtilConfig.LocalSourceDirectories);

            foreach (var line in File.ReadAllLines(ManifestPath))
            {
                if (line.Contains($"\"{repository.PackageIdentifier}\""))
                {
                    currentMode = line.Contains("file:") ? Mode.LocalFolder : Mode.GitHub;
                }
            }
        }

        private static void SwitchRepositoryTo(Mode newMode)
        {
            GetRepository(out PackageUtilConfig.Repository repository, out string packageLocalPath, out Mode currentMode);

            StringBuilder newFileContents = new StringBuilder();
            foreach (var line in File.ReadAllLines(ManifestPath))
            {
                if (line.Contains($"\"{repository.PackageIdentifier}\""))
                {
                    int colonIndex = line.IndexOf(":");
                    newFileContents.Append(line.Substring(0, colonIndex + 1));

                    if (newMode == Mode.LocalFolder)
                    {
                        newFileContents.AppendLine($" \"file:{packageLocalPath}\",");
                    }
                    else
                    {
                        newFileContents.AppendLine($" \"{repository.GitHubUrl}#{GetLastestGitHash(packageLocalPath)}\",");
                    }
                }
                else
                {
                    newFileContents.AppendLine(line);
                }
            }

            Debug.Log(newFileContents.ToString());
        }

        private static string GetLastestGitHash(string gitPath)
        {
            var gitProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "log",
                    WorkingDirectory = gitPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                }
            };

            gitProcess.Start();
            while (gitProcess.StandardOutput.EndOfStream == false)
            {
                return gitProcess.StandardOutput.ReadLine()
                    .Replace("commit ", "")
                    .Substring(0, 40);
            }

            return null;
        }

        private static string GetPackageName()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path.StartsWith(PackagesPath) && Directory.Exists(path) && path.IndexOf("/") == path.LastIndexOf("/"))
            {
                return path.Substring(PackagesPath.Length);
            }

            return null;
        }

        public static string GetPackageLocalPath(string packageName, List<string> packagesDirectories)
        {
            // Going through each one and moving the files if found
            foreach (var packagesDirectory in packagesDirectories)
            {
                string directory = packagesDirectory.Replace("\\", "/");
                directory = directory.EndsWith("/") ? directory.Substring(0, directory.Length - 1) : directory;

                string fullPackageDirectoryPathDesintation = Path.Combine(directory, packageName).Replace("\\", "/");

                if (Directory.Exists(fullPackageDirectoryPathDesintation))
                {
                    return fullPackageDirectoryPathDesintation;
                }
            }

            return null;
        }
    }
}
