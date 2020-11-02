//-----------------------------------------------------------------------
// <copyright file="MovePackageCode.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class MovePackageCode
    {
        private const string PackageDiretcoriesFile = ".packagedirectories";
        private const string PackagesPath = "Packages/";

        [MenuItem("Assets/Lost/Move Package Code", true)]
        public static bool MoveValidate()
        {
            return GetPackageName() != null;
        }

        [MenuItem("Assets/Lost/Move Package Code", false)]
        public static void Move()
        {
            string packagePath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string packageName = GetPackageName();

            if (File.Exists(PackageDiretcoriesFile) == false)
            {
                Debug.LogError($"{PackageDiretcoriesFile} not present!");
                return;
            }

            // Getting the list of valid package source locations
            List<string> packagesDirectories = new List<string>();

            foreach (var line in File.ReadAllLines(PackageDiretcoriesFile))
            {
                string directory = line.Trim().Replace("\\", "/");
                if (directory.IsNullOrWhitespace() == false)
                {
                    packagesDirectories.Add(directory);
                }
            }

            // Going through each one and moving the files if found
            foreach (var packagesDirectory in packagesDirectories)
            {
                string directory = packagesDirectory.Replace("\\", "/");
                directory = directory.EndsWith("/") ? directory.Substring(0, directory.Length - 1) : directory;

                string fullPackageDirectoryPathDesintation = Path.Combine(directory, packageName).Replace("\\", "/");
                string packageDirectorySource = (new FileInfo(packagePath)).FullName.Replace("\\", "/");

                if (Directory.Exists(fullPackageDirectoryPathDesintation))
                {
                    MoveFiles(packageDirectorySource, fullPackageDirectoryPathDesintation);
                }
            }
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

        private static void MoveFiles(string sourceDirectory, string destinationDirectory)
        {
            sourceDirectory = sourceDirectory.AppendIfDoesntExist("/");
            destinationDirectory = destinationDirectory.AppendIfDoesntExist("/");

            List<string> sourceFilesList = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories).Select(x => x.Substring(sourceDirectory.Length)).ToList();
            HashSet<string> sourceFilesHash = sourceFilesList.ToHashSet();

            List<string> destinationFilesList = Directory.GetFiles(destinationDirectory, "*", SearchOption.AllDirectories).Select(x => x.Substring(destinationDirectory.Length)).ToList();
            HashSet<string> destinationFilesHash = destinationFilesList.ToHashSet();

            foreach (var sourceFile in sourceFilesList)
            {
                string sourcePath = Path.Combine(sourceDirectory, sourceFile).Replace("\\", "/");
                string destPath = Path.Combine(destinationDirectory, sourceFile).Replace("\\", "/");

                CreateOrOverwriteFile(sourcePath, destPath);
            }

            foreach (var destFile in destinationFilesList)
            {
                string sourcePath = Path.Combine(sourceDirectory, destFile).Replace("\\", "/");
                string destPath = Path.Combine(destinationDirectory, destFile).Replace("\\", "/");

                if (destPath.Contains("/.git/") || destPath.Contains("/.svn/") || destPath.Contains("/.plastic/"))
                {
                    continue;
                }

                if (File.Exists(sourcePath) == false)
                {
                    DeleteFile(destPath);
                }
            }

            Debug.Log($"Finished Moveing {sourceDirectory} to {destinationDirectory}");
        }

        private static void CreateOrOverwriteFile(string source, string destination)
        {
            bool isCSharpFile = source.ToUpperInvariant().EndsWith(".CS");
            bool isMetaFile = source.ToUpperInvariant().EndsWith(".META");
            bool isAssemblyDef = source.ToUpperInvariant().EndsWith(".ASMDEF");
            byte[] sourceFileBytes;

            if (isCSharpFile)
            {
                sourceFileBytes = FileUtil.GetUtf8Bytes(GetCSharpFileContents(source));
            }
            else if (isMetaFile)
            {
                sourceFileBytes = GetTextFileContents(source).GetUTF8Bytes();
            }
            else if (isAssemblyDef)
            {
                sourceFileBytes = GetTextFileContents(source).GetUTF8Bytes();
            }
            else
            {
                sourceFileBytes = File.ReadAllBytes(source);
            }

            if (File.Exists(destination))
            {
                byte[] destBytes = File.ReadAllBytes(destination);

                if (AreByteArraysEqual(sourceFileBytes, destBytes) == false)
                {
                    Debug.Log($"Overwriting File: {destination}");
                    File.WriteAllBytes(destination, sourceFileBytes);
                }
            }
            else
            {
                // Making sure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(destination));

                Debug.Log($"Creating File: {destination}");
                File.WriteAllBytes(destination, sourceFileBytes);
            }
        }

        private static void DeleteFile(string destination)
        {
            Debug.Log($"Deleting File: {destination}");
        }

        public static string GetTextFileContents(string filePath)
        {
            return FileUtil.ConvertLineEndings(File.ReadAllText(filePath), LineEndingsMode.Unix);
        }

        public static string GetCSharpFileContents(string filePath)
        {
            string fileContents = GetTextFileContents(filePath);
            fileContents = FileUtil.TrimTrailingWhitespace(fileContents, '\n');
            fileContents = FileUtil.InsertFinalNewLine(fileContents, '\n');
            fileContents = FileUtil.ReplaceHardTabsWithSoftTabs(fileContents);

            return fileContents;
        }

        private static bool AreByteArraysEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
