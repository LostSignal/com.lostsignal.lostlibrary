﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LostLibrarySettings.cs" company="Lost Signal">
//     Copyright (c) Lost Signal. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public class LostLibrarySettings : ScriptableObject
    {
        public enum LineEndings
        {
            Unix,
            Windows,
        }

        public enum SourceControlType
        {
            Plastic,
            Perforce,
            Git,
            Collab,
        }

        public const string InstanceName = "Lost Library Settings";
        public const string SettingsWindowPath = "Project/Lost Library";
        public const string SettingsFilePath = "ProjectSettings/LostLibrarySettings.asset";

        static LostLibrarySettings()
        {
            EditorApplication.delayCall += Initialize;
        }

        private static void Initialize()
        {
            var settings = LostLibrarySettings.Instance;

            // Make sure Line Endings are set
            if (EditorSettings.lineEndingsForNewScripts != Convert(settings.projectLineEndings))
            {
                EditorSettings.lineEndingsForNewScripts = Convert(settings.projectLineEndings);
            }

            // Make sure Serialization Type is set
            if (settings.forceSerializationMode && EditorSettings.serializationMode != settings.serializationMode)
            {
                EditorSettings.serializationMode = settings.serializationMode;
            }

            // Make sure editorconfig exists
            if (settings.useEditorConfig && settings.editorConfigFileName.IsNullOrWhitespace() == false && File.Exists(settings.editorConfigFileName) == false)
            {
                File.WriteAllText(settings.editorConfigFileName, settings.editorConfigTemplate.text);
            }

            // Auto set p4 environment variable
            settings.AutoSetP4IgnoreEnvironmentVariable();

            // Auto set PlasticSCM settings
            settings.AutoSetPlasticSCMSettings();

            LineEndingsMode Convert(LineEndings lineEndings)
            {
                switch (lineEndings)
                {
                    case LineEndings.Unix:
                        return LineEndingsMode.Unix;

                    case LineEndings.Windows:
                        return LineEndingsMode.Windows;

                    default:
                        Debug.LogErrorFormat("Found unknown line endings type {0}", lineEndings);
                        return LineEndingsMode.Unix;
                }
            }
        }

        private static LostLibrarySettings instance;

        public static LostLibrarySettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = ScriptableObject.CreateInstance<LostLibrarySettings>();
                    instance.Load();
                }

                return instance;
            }
        }

        #pragma warning disable 0649
        // Line Endings and Serialization
        [SerializeField] private LineEndings projectLineEndings;
        [SerializeField] private bool forceSerializationMode;
        [SerializeField] private SerializationMode serializationMode;

        // Source Control Ignore File
        [SerializeField] private SourceControlType sourceControlType;
        [SerializeField] private TextAsset ignoreTemplateGit;
        [SerializeField] private TextAsset ignoreTemplateCollab;
        [SerializeField] private TextAsset ignoreTemplatePlastic;
        [SerializeField] private TextAsset ignoreTemplateP4;
        [SerializeField] private string p4IgnoreFileName;
        [SerializeField] private bool autosetP4IgnoreEnvironmentVariable;

        // Editorconfig
        [SerializeField] private bool useEditorConfig;
        [SerializeField] private string editorConfigFileName;
        [SerializeField] private TextAsset editorConfigTemplate;

        // Template File Overriding
        [SerializeField] private bool overrideTemplateFiles;
        [SerializeField] private TextAsset templateMonoBehaviour;
        [SerializeField] private TextAsset templatePlayableAsset;
        [SerializeField] private TextAsset templatePlayableBehaviour;
        [SerializeField] private TextAsset templateStateMachineBehaviour;
        [SerializeField] private TextAsset templateSubStateMachineBehaviour;
        [SerializeField] private TextAsset templateEditorTestScript;

        // PlasticSCM Settings
        [SerializeField] private bool plasticAutoSetFileCasingError;
        [SerializeField] private bool plasticAutoSetYamlMergeToolPath;

        // Analyzers
        [SerializeField] private List<Analyzer> analyzers;
        #pragma warning restore 0649

        public void Save()
        {
            if (instance == null)
            {
                Debug.Log("Can't save LostLibrarySettings, no instance can be found!");
                return;
            }

            if (string.IsNullOrEmpty(SettingsFilePath) == false)
            {
                string directoryName = Path.GetDirectoryName(SettingsFilePath);

                if (Directory.Exists(directoryName) == false)
                {
                    Directory.CreateDirectory(directoryName);
                }

                File.WriteAllText(SettingsFilePath, EditorJsonUtility.ToJson(instance, true));
            }
        }

        public void Load()
        {
            if (File.Exists(SettingsFilePath) == false)
            {
                this.LoadDefaults();
            }
            else
            {
                try
                {
                    var fileData = File.ReadAllText(SettingsFilePath);
                    EditorJsonUtility.FromJsonOverwrite(fileData, instance);
                }
                catch (Exception xcp)
                {
                    // Quash the exception and take the default settings.
                    Debug.LogException(xcp);
                    this.LoadDefaults();
                }
            }
        }

        public void LoadDefaults()
        {
            // Line Endings and Serialization
            this.projectLineEndings = LineEndings.Unix;
            this.forceSerializationMode = true;
            this.serializationMode = SerializationMode.ForceText;

            // Source Control Ignore File
            this.sourceControlType = SourceControlType.Plastic;
            this.ignoreTemplateP4 = EditorUtil.GetAssetByGuid<TextAsset>("6d6c8d3e6aeaff34d89c7f2be0a80a0d");
            this.ignoreTemplateGit = EditorUtil.GetAssetByGuid<TextAsset>("fae63426d3cf11c4cb39244488e2ec17");
            this.ignoreTemplateCollab = EditorUtil.GetAssetByGuid<TextAsset>("075673ae8dd02af42b6e15b9f718e0a7");
            this.ignoreTemplatePlastic = EditorUtil.GetAssetByGuid<TextAsset>("aafcbe005eaa6754b921e846efb9043d");
            this.p4IgnoreFileName = ".p4ignore";
            this.autosetP4IgnoreEnvironmentVariable = true;

            // Template File Overriding
            this.overrideTemplateFiles = true;
            this.templateMonoBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("5ec2f7fdcef1e6f45b2c1a7510be3eaa");
            this.templatePlayableAsset = EditorUtil.GetAssetByGuid<TextAsset>("e4d5fd6d65c83d24da92fbd00d7f5499");
            this.templatePlayableBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("6ccc7dcc8373b7f4197de5cd7d7e7a16");
            this.templateStateMachineBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("fed9948eb87d1be48ae323bd48cf729f");
            this.templateSubStateMachineBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("09afd0c31b0565e4a8a74ecb68ceef24");
            this.templateEditorTestScript = EditorUtil.GetAssetByGuid<TextAsset>("c31e8a34fb6708144809d22dffdc73f6");

            // Editorconfig
            this.useEditorConfig = true;
            this.editorConfigFileName = ".editorconfig";
            this.editorConfigTemplate = EditorUtil.GetAssetByGuid<TextAsset>("f6c774b1ff43524428c88bc6afaca2d7");

            // PlasticSCM Settings
            this.plasticAutoSetFileCasingError = true;
            this.plasticAutoSetYamlMergeToolPath = true;

            // Analyzers
            this.analyzers = new List<Analyzer>
            {
                new Analyzer()
                {
                    Name = "StyleCop",
                    Ruleset = EditorUtil.GetAssetByGuid<TextAsset>("6d22bf8a5b4217246a8bd27939b3a093"),
                    Config = EditorUtil.GetAssetByGuid<TextAsset>("447a0d2defa062a4cb1ab9f0a161d7f7"),
                    DLLs = new List<TextAsset>
                    {
                        EditorUtil.GetAssetByGuid<TextAsset>("34b2bcdbab6772c43803d97146553550"),
                        EditorUtil.GetAssetByGuid<TextAsset>("fdf22cdd44a87ed4f9ae0c0d6e685ae6"),
                        EditorUtil.GetAssetByGuid<TextAsset>("d86a7268d4b5874478f3bf9019de4dd3"),
                    },
                    CSProjects = new List<string>
                    {
                        // "Assembly-CSharp",
                        // "Assembly-CSharp.Player",
                        // "Assembly-CSharp-Editor",
                        "LostLibrary",
                        "LostLibrary.Editor",
                        "LostLibrary.LBE",
                        "LostLibrary.LBE.Player",
                        "LostLibrary.Player",
                        "LostLibrary.Test",
                    },
                }
            };
        }

        public void OverrideCSharpTemplateFiles(string assetPath)
        {
            if (assetPath.EndsWith(".cs.meta") == false)
            {
                return;
            }

            // Getting the full asset path by removing the ".meta" extension
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf("."));

            // Getting the new template files
            TextAsset templateFile = this.GetTemplateTextAsset(assetPath);

            if (templateFile == null)
            {
                return;
            }

            // Determining the company name and namespace
            bool isLostFolder = assetPath.StartsWith("Packages/com.lostsignal.lostlibrary/");
            string companyName = "Lost Signal LLC";
            string nameSpace = "Lost";

            if (isLostFolder == false)
            {
                companyName = string.IsNullOrWhiteSpace(PlayerSettings.companyName) ? "Player Settings Company Not Defined" : PlayerSettings.companyName;
                nameSpace = string.IsNullOrWhiteSpace(EditorSettings.projectGenerationRootNamespace) ? "EditorSettingsRootNamespaceNotDefined" : EditorSettings.projectGenerationRootNamespace;
            }

            // Getting the script name and the template file to use
            string scriptName = Path.GetFileNameWithoutExtension(assetPath);

            // Writing the C# File
            string fileContents = templateFile == null ? File.ReadAllText(assetPath) : templateFile.text;

            fileContents = fileContents.Replace("#COMPANY_NAME#", companyName)
                .Replace("#ROOTNAMESPACE#", nameSpace)
                .Replace("#SCRIPTNAME#", scriptName)
                .Replace("#NOTRIM#", string.Empty);

            File.WriteAllText(assetPath, FileUtil.ConvertLineEndings(fileContents));
            AssetDatabase.Refresh();
        }

        public void AutoSetP4IgnoreEnvironmentVariable()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor &&
                this.sourceControlType == SourceControlType.Perforce &&
                this.autosetP4IgnoreEnvironmentVariable &&
                this.p4IgnoreFileName != GetCurrentP4IgnoreVariableWindows())
            {
                SetP4IgnoreVariableForWindows(this.p4IgnoreFileName);
            }

            string GetCurrentP4IgnoreVariableWindows()
            {
                try
                {
                    var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "p4",
                        Arguments = "set P4IGNORE",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    });

                    return process.StandardOutput.ReadToEnd().Replace("P4IGNORE=", string.Empty).Replace("(set)", string.Empty).Trim();
                }
                catch
                {
                    return null;
                }
            }

            void SetP4IgnoreVariableForWindows(string p4ignoreFileName)
            {
                try
                {
                    System.Diagnostics.Process.Start("p4", "set P4IGNORE=" + p4ignoreFileName);
                }
                catch
                {
                    Debug.LogError("Unable To Set P4IGNORE Variable.  Is P4 installed?");
                }
            }
        }

        public void AutoSetPlasticSCMSettings()
        {
            if (this.sourceControlType != SourceControlType.Plastic)
            {
                return;
            }

            PlasticSCM.UpdateClientConfigSettings(PlasticSCM.GetClientConfigPath(), this.plasticAutoSetFileCasingError, this.plasticAutoSetYamlMergeToolPath);
        }

        public void GenerateSourceControlIgnoreFile()
        {
            if (this.sourceControlType == SourceControlType.Plastic)
            {
                var currentUnityDirectoryInfo = new DirectoryInfo(".");
                var currentUnityDirectoryPath = currentUnityDirectoryInfo.FullName.Replace("\\", "/");
                var plasticDirectoryPath = FindPlasticRootDirectoryPath(currentUnityDirectoryInfo);

                if (string.IsNullOrEmpty(plasticDirectoryPath))
                {
                    Debug.LogError("Unable to find the root of the Plastic repository.  File was not created.");
                    return;
                }

                string relativeUnityDirectory = currentUnityDirectoryPath != plasticDirectoryPath ?
                    "/" + currentUnityDirectoryPath.Substring(plasticDirectoryPath.Length + 1).Replace("\\", "/") :
                    string.Empty;

                File.WriteAllText(
                    Path.Combine(plasticDirectoryPath, "ignore.conf"),
                    this.ignoreTemplatePlastic.text.Replace("{UNITY_PROJECT_DIRECTORY}", relativeUnityDirectory));
            }
            else if (this.sourceControlType == SourceControlType.Perforce)
            {
                File.WriteAllText(this.p4IgnoreFileName, this.ignoreTemplateP4.text);
            }
            else if (this.sourceControlType == SourceControlType.Git)
            {
                File.WriteAllText(".gitignore", this.ignoreTemplateGit.text);
            }
            else if (this.sourceControlType == SourceControlType.Collab)
            {
                File.WriteAllText(".collabignore", this.ignoreTemplateCollab.text);
            }
        }

        private string FindPlasticRootDirectoryPath(DirectoryInfo directory)
        {
            string directoryPath = directory.FullName.Replace("\\", "/");

            if (Directory.Exists(Path.Combine(directoryPath, ".plastic")))
            {
                return directoryPath;
            }
            else if (string.IsNullOrEmpty(directory.Parent?.FullName) == false)
            {
                return FindPlasticRootDirectoryPath(directory.Parent);
            }
            else
            {
                return null;
            }
        }

        public void AddEditorConfigToSolution()
        {
            if (this.useEditorConfig == false)
            {
                return;
            }

            foreach (var solutionFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln"))
            {
                FixSolution(solutionFile);
            }

            void FixSolution(string solutionFilePath)
            {
                string content = File.ReadAllText(solutionFilePath);

                if (File.Exists(this.editorConfigFileName) && content.Contains(this.editorConfigFileName) == false)
                {
                    File.WriteAllText(solutionFilePath, content.Insert(content.IndexOf("Global"), GetEditorconfigString()));
                }
            }

            static string GetEditorconfigString()
            {
                var builder = new StringBuilder();
                builder.AppendLine("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", \"{NEW_GUID}\"");
                builder.AppendLine("\tProjectSection(SolutionItems) = preProject");
                builder.AppendLine("\t\t.editorconfig = .editorconfig");
                builder.AppendLine("\tEndProjectSection");
                builder.AppendLine("EndProject");

                return builder.ToString().Replace("NEW_GUID", Guid.NewGuid().ToString().ToUpper());
            }
        }

        public void AddAnalyzersToCSProjects()
        {
            foreach (var csProjFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj"))
            {
                if (this.analyzers == null || this.analyzers.Count == 0)
                {
                    continue;
                }

                for (int i = 0; i < this.analyzers.Count; i++)
                {
                    var analyzer = this.analyzers[i];
                    var fileName = Path.GetFileNameWithoutExtension(csProjFile);

                    if (analyzer.CSProjects.Contains(fileName))
                    {
                        this.AddAnalyzerToCSProj(analyzer, csProjFile, i);
                    }
                }
            }
        }

        [MenuItem("Tools/AddAnalyzersToCSProjects")]
        public static void RunAnalyzerCode()
        {
            LostLibrarySettings.instance.AddAnalyzersToCSProjects();
        }

        private TextAsset GetTemplateTextAsset(string assetPath)
        {
            if (this.overrideTemplateFiles == false)
            {
                return null;
            }

            string fileContents = File.ReadAllText(assetPath);

            if (fileContents.Contains(": PlayableAsset"))
            {
                return this.templatePlayableAsset;
            }
            else if (fileContents.Contains(": PlayableBehaviour"))
            {
                return this.templatePlayableBehaviour;
            }
            else if (fileContents.Contains(": StateMachineBehaviour"))
            {
                if (fileContents.Contains("OnStateMachineEnter"))
                {
                    return this.templateSubStateMachineBehaviour;
                }
                else
                {
                    return this.templateStateMachineBehaviour;
                }
            }
            else if (fileContents.Contains("[Test]"))
            {
                return this.templateEditorTestScript;
            }
            else if (fileContents.Contains(": MonoBehaviour"))
            {
                return this.templateMonoBehaviour;
            }

            return null;
        }

        private void AddAnalyzerToCSProj(Analyzer analyzer, string csprojFilePath, int analyzerIndex)
        {
            var additionalFiles = new List<string>();
            var ruleSets = new List<string>();
            var analyzers = new List<string>();

            if (analyzer.Config != null)
            {
                additionalFiles.Add(FullPath(analyzer.Config));
            }

            if (analyzer.Ruleset != null)
            {
                ruleSets.Add(FullPath(analyzer.Ruleset));
            }

            if (analyzer.DLLs != null)
            {
                for (int i = 0; i < analyzer.DLLs.Count; i++)
                {
                    analyzers.Add(CreateDLL(analyzer.DLLs[i]));
                }
            }

            CSProjHelper.UpdateCSProjFile(csprojFilePath, additionalFiles, ruleSets, analyzers);

            string CreateDLL(TextAsset dllAsset)
            {
                var sourceFilePath = FullPath(dllAsset);
                var sourceFileBytes = File.ReadAllBytes(sourceFilePath);

                string dllDirectory = $"./Library/Analyzers/{analyzerIndex}_{analyzer.Name}";

                if (Directory.Exists(dllDirectory) == false)
                {
                    Directory.CreateDirectory(dllDirectory);
                }

                string dllName = Path.GetFileNameWithoutExtension(sourceFilePath);
                string fullDllPath = Path.GetFullPath(Path.Combine(dllDirectory, dllName));

                if (File.Exists(fullDllPath))
                {
                    File.Delete(fullDllPath);
                }

                File.WriteAllBytes(fullDllPath, sourceFileBytes);

                return fullDllPath;
            }

            string FullPath(UnityEngine.Object obj) => Path.GetFullPath(AssetDatabase.GetAssetPath(obj));
        }

        public static class CSProjHelper
        {
            public static void UpdateCSProjFile(string csprojFilePath, List<string> additionalFiles, List<string> rulesetFiles, List<string> analyzerDlls)
            {
                var lines = File.ReadAllLines(csprojFilePath).ToList();

                AddRulesets(rulesetFiles, lines);
                AddAnalyzerDLLs(analyzerDlls, lines);
                AddAdditionalFiles(additionalFiles, lines);

                File.WriteAllText(csprojFilePath, GetFileContents(lines));

                void AddRulesets(List<string> files, List<string> lines)
                {
                    if (files == null || files.Count == 0)
                    {
                        return;
                    }

                    int rulesetIndex = GetLineIndex("<CodeAnalysisRuleSet>", lines);
                    int firstItemGroupIndex = GetLineIndex("<ItemGroup>", lines);
                    var newLines = new List<string>();
                    var newLinesAdded = false;

                    if (rulesetIndex == -1)
                    {
                        newLines.Add("  <PropertyGroup>");
                    }

                    foreach (var file in files)
                    {
                        string newLine = $"   <CodeAnalysisRuleSet>{file}</CodeAnalysisRuleSet>";

                        if (lines.Contains(newLine) == false)
                        {
                            newLinesAdded = true;
                            newLines.Add(newLine);
                        }
                    }

                    if (rulesetIndex == -1)
                    {
                        newLines.Add("  </PropertyGroup>");
                    }

                    if (newLinesAdded)
                    {
                        lines.InsertRange(rulesetIndex != -1 ? rulesetIndex + 1 : firstItemGroupIndex, newLines);
                    }
                }

                void AddAdditionalFiles(List<string> files, List<string> lines)
                {
                    if (files == null || files.Count == 0)
                    {
                        return;
                    }

                    int additionalFilesIndex = GetLineIndex("<AdditionalFiles ", lines);
                    int firstItemGroupIndex = GetLineIndex("<ItemGroup>", lines);
                    var newLines = new List<string>();
                    var newLinesAdded = false;

                    if (additionalFilesIndex == -1)
                    {
                        newLines.Add("  <ItemGroup>");
                    }

                    foreach (var file in files)
                    {
                        string newLine = $"    <AdditionalFiles Include=\"{file}\" />";

                        if (lines.Contains(newLine) == false)
                        {
                            newLinesAdded = true;
                            newLines.Add(newLine);
                        }
                    }

                    if (additionalFilesIndex == -1)
                    {
                        newLines.Add("  </ItemGroup>");
                    }

                    if (newLinesAdded)
                    {
                        lines.InsertRange(additionalFilesIndex != -1 ? additionalFilesIndex + 1 : firstItemGroupIndex, newLines);
                    }
                }

                void AddAnalyzerDLLs(List<string> files, List<string> lines)
                {
                    if (files == null || files.Count == 0)
                    {
                        return;
                    }

                    int analyzerFilesIndex = GetLineIndex("<Analyzer ", lines);
                    int firstItemGroupIndex = GetLineIndex("<ItemGroup>", lines);
                    var newLines = new List<string>();
                    var newLinesAdded = false;

                    if (analyzerFilesIndex == -1)
                    {
                        newLines.Add("  <ItemGroup>");
                    }

                    foreach (var file in files)
                    {
                        string newLine = $"    <Analyzer Include=\"{file}\" />";

                        if (lines.Contains(newLine) == false)
                        {
                            newLinesAdded = true;
                            newLines.Add(newLine);
                        }
                    }

                    if (analyzerFilesIndex == -1)
                    {
                        newLines.Add("  </ItemGroup>");
                    }

                    if (newLinesAdded)
                    {
                        lines.InsertRange(analyzerFilesIndex != -1 ? analyzerFilesIndex + 1 : firstItemGroupIndex, newLines);
                    }
                }

                int GetLineIndex(string startsWith, List<string> lines)
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i] != null && lines[i].Trim().StartsWith(startsWith))
                        {
                            return i;
                        }
                    }

                    return -1;
                }

                string GetFileContents(List<string> lines)
                {
                    StringBuilder result = new StringBuilder();

                    foreach (var line in lines)
                    {
                        result.AppendLine(line);
                    }

                    return result.ToString();
                }
            }
        }

        [Serializable]
        public class Analyzer
        {
            [SerializeField] private string name;
            [SerializeField] private TextAsset ruleset;
            [SerializeField] private TextAsset config;
            [SerializeField] private List<TextAsset> dlls;
            [SerializeField] private List<string> csProjects;

            public string Name
            {
                get => this.name;
                set => this.name = value;
            }

            public List<TextAsset> DLLs
            {
                get => this.dlls;
                set => this.dlls = value;
            }

            public TextAsset Ruleset
            {
                get => this.ruleset;
                set => this.ruleset = value;
            }

            public TextAsset Config
            {
                get => this.config;
                set => this.config = value;
            }

            public List<string> CSProjects
            {
                get => this.csProjects;
                set => this.csProjects = value;
            }
        }
    }
}
