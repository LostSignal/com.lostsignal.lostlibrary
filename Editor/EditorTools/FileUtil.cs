//-----------------------------------------------------------------------
// <copyright file="FileUtil.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public static class FileUtil
    {
        public static void CreateOrUpdateFile(string contents, string path, bool useSourceControl, LineEndingsMode lineEndings)
        {
            if (File.Exists(path))
            {
                UpdateFile(contents, path, useSourceControl, lineEndings);
            }
            else
            {
                CreateFile(contents, path, useSourceControl, lineEndings);
            }
        }

        public static void CreateOrUpdateFile(string contents, string path, bool useSourceControl)
        {
            CreateOrUpdateFile(contents, path, useSourceControl, EditorSettings.lineEndingsForNewScripts);
        }

        public static void CreateFile(string contents, string destinationFile, bool sourceControlAdd, LineEndingsMode lineEndings)
        {
            string fileContents = ConvertLineEndings(contents, lineEndings);

            // Actually writing out the contents
            File.WriteAllText(destinationFile, fileContents);

            // Telling source control to add the file
            if (sourceControlAdd && Provider.enabled && Provider.isActive)
            {
                AssetDatabase.Refresh();
                Provider.Add(new Asset(destinationFile), false).Wait();
            }
        }

        public static void CreateFile(string contents, string destinationFile, bool sourceControlAdd)
        {
            CreateFile(contents, destinationFile, sourceControlAdd, EditorSettings.lineEndingsForNewScripts);
        }

        public static void UpdateFile(string contents, string path, bool useSourceControl, LineEndingsMode lineEndings)
        {
            string fileContents = ConvertLineEndings(contents, lineEndings);

            // Early out if nothing has changed
            if (File.ReadAllText(path) == fileContents)
            {
                return;
            }

            // Checking out the file
            if (useSourceControl && Provider.enabled && Provider.isActive)
            {
                Provider.Checkout(path, CheckoutMode.Asset).Wait();
            }

            File.WriteAllText(path, fileContents);
        }

        public static void UpdateFile(string contents, string path, bool useSourceControl)
        {
            UpdateFile(contents, path, useSourceControl, EditorSettings.lineEndingsForNewScripts);
        }

        public static void CopyFile(string sourceFile, string destinationFile, bool sourceControlCheckout, LineEndingsMode lineEndings)
        {
            if (File.Exists(sourceFile) == false)
            {
                Debug.LogErrorFormat("Unable to copy file {0} to {1}.  Source file does not exist!", sourceFile, destinationFile);
            }

            string fileContents = ConvertLineEndings(File.ReadAllText(sourceFile), lineEndings);

            if (fileContents != File.ReadAllText(destinationFile))
            {
                // checking out the file
                if (sourceControlCheckout && Provider.enabled && Provider.isActive)
                {
                    Provider.Checkout(destinationFile, CheckoutMode.Asset).Wait();
                }

                // actually writing out the contents
                File.WriteAllText(destinationFile, fileContents);
            }
        }

        public static void CopyFile(string sourceFile, string destinationFile, bool sourceControlCheckout)
        {
            CopyFile(sourceFile, destinationFile, sourceControlCheckout, EditorSettings.lineEndingsForNewScripts);
        }

        public static string ConvertLineEndings(string inputText, LineEndingsMode lineEndings)
        {
            // checking for a really messed up situation that happens when mixing max/pc sometimes
            if (inputText.Contains("\r\r\n"))
            {
                inputText = inputText.Replace("\r\r\n", "\n");
            }

            // if it has windows line escaping, then convert everything to Unix
            if (inputText.Contains("\r\n"))
            {
                inputText = inputText.Replace("\r\n", "\n");
            }

            if (lineEndings == LineEndingsMode.Unix)
            {
                // do nothing, already in Unix
            }
            else if (lineEndings == LineEndingsMode.Windows)
            {
                // convert all unix to windows
                inputText = inputText.Replace("\n", "\r\n");
            }
            else if (lineEndings == LineEndingsMode.OSNative)
            {
                // convert all os native to windows
                inputText = inputText.Replace("\n", System.Environment.NewLine);
            }
            else
            {
                Debug.LogErrorFormat("Unable to convert line endings, unknown line ending type found: {0}", lineEndings);
            }

            return inputText;
        }

        public static string TrimTrailingWhitespace(string fileContents, char newlineCharacter)
        {
            StringBuilder newFileContents = new StringBuilder();

            foreach (var line in fileContents.Split(newlineCharacter))
            {
                newFileContents.Append(line.TrimEnd());
                newFileContents.Append(newlineCharacter);
            }

            return newFileContents.ToString();
        }

        public static string ReplaceHardTabsWithSoftTabs(string fileContents)
        {
            return fileContents.Replace("\t", "    ");
        }

        public static string InsertFinalNewLine(string fileContents, char newlineCharacter)
        {
            return fileContents.TrimEnd() + newlineCharacter;
        }

        public static string ConvertLineEndings(string inputText)
        {
            return ConvertLineEndings(inputText, EditorSettings.lineEndingsForNewScripts);
        }

        public static byte[] GetUtf8Bytes(string fileContents)
        {
            byte[] contents = new UTF8Encoding().GetBytes(fileContents);
            MemoryStream finalFileContents = new MemoryStream();
            finalFileContents.Write(contents, 0, contents.Length);
            return finalFileContents.ToArray();
        }

        public static void RemoveEmptyDirectories(string directory)
        {
            foreach (var childDirectory in Directory.GetDirectories(directory))
            {
                RemoveEmptyDirectories(childDirectory);
            }

            if (Directory.GetDirectories(directory).Length == 0 && Directory.GetFiles(directory).Length == 0)
            {
                Debug.LogFormat("Removing Directory: {0}", directory);
                AssetDatabase.MoveAssetToTrash(directory);
            }
        }
    }
}
