//-----------------------------------------------------------------------
// <copyright file="CSharpProjectGenerator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Lost/CSharp Project Generator")]
    public class CSharpProjectGenerator : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private string projectName;
        [SerializeField] private string folderPath;
        [SerializeField] private TextObject csProjectFileContent;
        [SerializeField] private TextObject slnProjectFileContent;
        [SerializeField] private string solutionGuid = Guid.NewGuid().ToString().ToUpper();
        [SerializeField] private string csProjectGuid = Guid.NewGuid().ToString().ToUpper();
        [SerializeField] private string[] defines = new string[0];
        [SerializeField] private TextObject[] files = new TextObject[0];
        [SerializeField] private CodeFolder[] codeFolders = new CodeFolder[0];
        [SerializeField] private Package[] packages = new Package[0];
        [SerializeField] private Replace[] customReplaceVariables = new Replace[0];
#pragma warning restore 0649

        public virtual string SolutionFilePath => this.GetFullFilePath(this.projectName + ".sln");

        public virtual string CsProjFilePath => this.GetFullFilePath(this.projectName + ".csproj");

        public string ProjectName => this.projectName;

        protected CodeFolder[] CodeFolders => this.codeFolders;

        [ExposeInEditor("Generate C# Project")]
        public virtual void Generate()
        {
            // Making sure the solution directory exists
            var solutionDirectory = Path.GetDirectoryName(this.SolutionFilePath);
            if (Directory.Exists(solutionDirectory) == false)
            {
                Directory.CreateDirectory(solutionDirectory);
            }

            this.WriteFile(this.SolutionFilePath, this.slnProjectFileContent.Text);
            this.WriteFile(this.CsProjFilePath, this.csProjectFileContent.Text);

            foreach (var file in this.files)
            {
                var filePath = this.GetFullFilePath(file.name);
                var fileDirectory = Path.GetDirectoryName(filePath);

                // Making sure file directory exists
                if (Directory.Exists(fileDirectory) == false)
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                this.WriteFile(filePath, file.Text);
            }
        }

        [ExposeInEditor("Open C# Project")]
        public virtual void Open()
        {
            if (System.IO.File.Exists(this.SolutionFilePath) == false)
            {
                this.Generate();
            }

            System.Diagnostics.Process.Start(this.SolutionFilePath);
        }

        protected virtual void WriteFile(string filePath, string contents)
        {
            contents = contents.Replace("__SLN_GUID__", this.solutionGuid)
                .Replace("__CS_PROJ_GUID__", this.csProjectGuid)
                .Replace("__PROJECT_NAME__", this.projectName)
                .Replace("__PROJECT_DEFINES__", this.GetDefines())
                .Replace("__PROJECT_CODE_DIRECTORIES__", this.GetDirectories())
                .Replace("__PROJECT_PACKAGES__", this.GetPackages());

            foreach (var customReplace in this.customReplaceVariables)
            {
                contents = contents.Replace(customReplace.ReplaceName, customReplace.ReplaceValue);
            }

            System.IO.File.WriteAllText(filePath, contents);
        }

        private string GetFullFilePath(string fileName)
        {
            return new FileInfo(Path.Combine(this.GetDirectory(), fileName)).FullName;
        }

        private string GetDirectory()
        {
            return new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), this.folderPath)).FullName;
        }

        private string GetDefines()
        {
            return this.defines.Length > 0 ?
                ";" + string.Join(";", this.defines) :
                string.Empty;
        }

        private string GetDirectories()
        {
            StringBuilder result = new StringBuilder();

            foreach (var codeFolder in this.codeFolders)
            {
                var assetPath = AssetDatabase.GetAssetPath(codeFolder.Folder);
                var absoluteAssetPath = Path.GetFullPath(assetPath);
                var absoluteAssetPathUri = new Uri(absoluteAssetPath);

                string codePath;

                try
                {
                    var absoluteCsharpFolderUri = new Uri(Path.GetFullPath(this.CsProjFilePath));
                    codePath = absoluteCsharpFolderUri.MakeRelativeUri(absoluteAssetPathUri).ToString();
                }
                catch
                {
                    codePath = absoluteAssetPath;
                }

                codePath = codePath.Replace("/", "\\");

                result.AppendLine($"    <Compile Include=\"{codePath}\\**\\*.cs\">");
                result.AppendLine($"      <Link>{codeFolder.FolderName}\\%(RecursiveDir)%(FileName)</Link>");
                result.Append("    </Compile>");

                if (codeFolder != this.codeFolders.Last())
                {
                    result.AppendLine();
                }
            }

            return result.ToString();
        }

        private string GetPackages()
        {
            if (this.packages.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            var lastPackage = this.packages.Last();

            foreach (var package in this.packages)
            {
                result.Append($"    <PackageReference Include=\"{package.PackageName}\" Version=\"{package.PackageVersion}\" />");

                if (package != lastPackage)
                {
                    result.AppendLine();
                }
            }

            return result.ToString();
        }

        [Serializable]
        public class File
        {
#pragma warning disable 0649
            [SerializeField] private string fileName;
            [SerializeField] private TextAsset fileContents;
#pragma warning restore 0649

            public string FileName => this.fileName;

            public TextAsset FileContents => this.fileContents;
        }

        [Serializable]
        public class CodeFolder
        {
#pragma warning disable 0649
            [SerializeField] private string folderName;
            [SerializeField] private DefaultAsset folder;
#pragma warning restore 0649

            public string FolderName => this.folderName;

            public DefaultAsset Folder => this.folder;
        }

        [Serializable]
        public class Package
        {
#pragma warning disable 0649
            [SerializeField] private string packageName;
            [SerializeField] private string packageVersion;
#pragma warning restore 0649

            public string PackageName => this.packageName;

            public string PackageVersion => this.packageVersion;
        }


        [Serializable]
        public class Replace
        {
#pragma warning disable 0649
            [SerializeField] private string replaceName;
            [SerializeField] private string replaceValue;
#pragma warning restore 0649

            public string ReplaceName => this.replaceName;

            public string ReplaceValue => this.replaceValue;
        }
    }
}
