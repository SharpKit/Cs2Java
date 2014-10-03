using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Specialized;
using System.Collections;
//using SharpKit.Build.Tasks.CsbService.Client;
using System.Configuration;
using JSharp.Compiler;
using cs2java.Tasks;
//using System.ServiceModel.Configuration;
//using System.ServiceModel;

namespace SharpKit.Build.Tools
{

    public class Cs2Java : ManagedCompiler
    {
        public Cs2Java()
        {
            UseBuildService = true;
        }

        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(ToolPath, ToolExe);
        }
        private static bool IsLegalIdentifier(string identifier)
        {
            if (identifier.Length == 0)
            {
                return false;
            }
            if (!TokenChar.IsLetter(identifier[0]) && (identifier[0] != '_'))
            {
                return false;
            }
            for (int i = 1; i < identifier.Length; i++)
            {
                char c = identifier[i];
                if (((!TokenChar.IsLetter(c) && !TokenChar.IsDecimalDigit(c)) && (!TokenChar.IsConnecting(c) && !TokenChar.IsCombining(c))) && !TokenChar.IsFormatting(c))
                {
                    return false;
                }
            }
            return true;
        }
        internal string GetDefineConstantsSwitch(string originalDefineConstants)
        {
            if (originalDefineConstants != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (string str in originalDefineConstants.Split(new char[] { ',', ';', ' ' }))
                {
                    if (IsLegalIdentifier(str))
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(";");
                        }
                        builder.Append(str);
                    }
                    else if (str.Length > 0)
                    {
                        base.Log.LogWarningWithCodeFromResources("Csc.InvalidParameterWarning", new object[] { "/define:", str });
                    }
                }
                if (builder.Length > 0)
                {
                    return builder.ToString();
                }
            }
            return null;
        }

        public ITaskItem[] NoneFiles { get; set; }
        public ITaskItem[] ContentFiles { get; set; }
        public ITaskItem[] SkcPlugins { get; set; }

        protected override void AddResponseFileCommands(CommandLineBuilderExtension commandLine)
        {
            if (OutputGeneratedFile != null && !String.IsNullOrEmpty(OutputGeneratedFile.ItemSpec))
                commandLine.AppendSwitchIfNotNull("/outputgeneratedfile:", OutputGeneratedFile);
            commandLine.AppendSwitchUnquotedIfNotNull("/define:", this.GetDefineConstantsSwitch(base.DefineConstants));
            this.AddReferencesToCommandLine(commandLine);
            base.AddResponseFileCommands(commandLine);
            if (ResponseFiles != null)
            {
                foreach (ITaskItem item in ResponseFiles)
                {
                    commandLine.AppendSwitchIfNotNull("@", item.ItemSpec);
                }
            }
            if (ContentFiles != null)
            {
                foreach (var file in ContentFiles)
                {
                    commandLine.AppendSwitchIfNotNull("/contentfile:", file.ItemSpec);
                }
            }
            if (NoneFiles != null)
            {
                foreach (var file in NoneFiles)
                {
                    commandLine.AppendSwitchIfNotNull("/nonefile:", file.ItemSpec);
                }
            }
            if (SkcPlugins != null)
            {
                foreach (var file in SkcPlugins)
                {
                    commandLine.AppendSwitchIfNotNull("/plugin:", file.ItemSpec);
                }
            }
            if (SkcRebuild)
                commandLine.AppendSwitch("/rebuild");
            if (UseBuildService)
            {
                Log.LogMessage("CurrentDirectory is: " + Directory.GetCurrentDirectory());
                commandLine.AppendSwitchIfNotNull("/dir:", Directory.GetCurrentDirectory());
            }

            commandLine.AppendSwitchIfNotNull("/TargetFrameworkVersion:", TargetFrameworkVersion);
        }
        private void AddReferencesToCommandLine(CommandLineBuilderExtension commandLine)
        {
            if ((base.References != null) && (base.References.Length != 0))
            {
                foreach (ITaskItem item in base.References)
                {
                    string metadata = item.GetMetadata("Aliases");
                    if ((metadata == null) || (metadata.Length == 0))
                    {
                        commandLine.AppendSwitchIfNotNull("/reference:", item.ItemSpec);
                    }
                    else
                    {
                        foreach (string str2 in metadata.Split(new char[] { ',' }))
                        {
                            string str3 = str2.Trim();
                            if (str2.Length != 0)
                            {
                                if (str3.IndexOfAny(new char[] { ',', ' ', ';', '"' }) != -1)
                                {
                                    throw new ArgumentException("Csc.AssemblyAliasContainsIllegalCharacters" + item.ItemSpec + str3);
                                }
                                if (string.Compare("global", str3, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    commandLine.AppendSwitchIfNotNull("/reference:", item.ItemSpec);
                                }
                                else
                                {
                                    throw new NotImplementedException("TODO: Implement");
                                    //commandLine.AppendSwitchAliased("/reference:", str3, item.ItemSpec);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            base.LogEventsFromTextOutput(singleLine, messageImportance);
        }

        protected override MessageImportance StandardOutputLoggingImportance
        {
            get
            {
                return MessageImportance.High;
            }
        }

        [Output]
        public ITaskItem OutputGeneratedFile { get; set; }

        private string GetTemporaryResponseFile(string responseFileCommands, out string responseFileSwitch)
        {
            string path = null;
            responseFileSwitch = null;
            if (!string.IsNullOrEmpty(responseFileCommands))
            {
                try
                {
                    path = Path.GetTempFileName();
                }
                catch (IOException exception)
                {
                    throw new IOException("Shared.FailedCreatingTempFile", exception);
                }
                using (StreamWriter writer = new StreamWriter(path, false, this.ResponseFileEncoding))
                {
                    writer.Write(responseFileCommands);
                }
                responseFileSwitch = this.GetResponseFileSwitch(path);
            }
            return path;
        }


        public bool UseHostCompilerIfAvailable { get; set; }

        public string TargetFrameworkVersion { get; set; }
        public bool UseBuildService { get; set; }
        public bool SkcRebuild { get; set; }

        string OutputGeneratedDir;
        string AssemblyFilenameWithoutExtension;
        public override bool Execute()
        {
            var outputAssembly = OutputAssembly.ItemSpec;
            OutputGeneratedDir = Path.GetDirectoryName(outputAssembly);
            AssemblyFilenameWithoutExtension = Path.GetFileNameWithoutExtension(outputAssembly);
            OutputAssembly.ItemSpec = Path.Combine(OutputGeneratedDir, Path.GetFileName(outputAssembly));
            if (UseBuildService)
            {
                var args = new CompilerToolArgs();
                AddResponseFileCommands2(args);
                var res = new CompilerServiceClient().Compile(new CompileRequest { Args = args });
                foreach (var s in res.Output)
                {
                    LogEventsFromTextOutput(s, MessageImportance.High);
                }
                return res.ExitCode == 0;
            }
            else
            {
                var success = base.Execute();
                return success;
            }
        }

        protected override bool CallHostObjectToExecute()
        {
            return base.CallHostObjectToExecute();
        }
        protected override string ToolName
        {
            get
            {
                return "skc.exe";
            }
        }
        void BaseAddResponseFileCommands(CompilerToolArgs args)
        {
            if (this.OutputAssembly == null && this.Sources != null && this.Sources.Length > 0 && this.ResponseFiles == null)
            {
                try
                {
                    this.OutputAssembly = new TaskItem(Path.GetFileNameWithoutExtension(this.Sources[0].ItemSpec));
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(ex.Message, "Sources");
                }
                if (string.Compare(this.TargetType, "library", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ITaskItem expr_79 = this.OutputAssembly;
                    expr_79.ItemSpec += ".dll";
                }
                else
                {
                    if (string.Compare(this.TargetType, "module", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ITaskItem expr_A9 = this.OutputAssembly;
                        expr_A9.ItemSpec += ".netmodule";
                    }
                    else
                    {
                        ITaskItem expr_C6 = this.OutputAssembly;
                        expr_C6.ItemSpec += ".exe";
                    }
                }
            }
            //        commandLine.AppendSwitchIfNotNull("/addmodule:", this.AddModules, ",");
            //        commandLine.AppendSwitchWithInteger("/codepage:", base.Bag, "CodePage");
            //        this.ConfigureDebugProperties();
            //        commandLine.AppendPlusOrMinusSwitch("/debug", base.Bag, "EmitDebugInformation");
            //        commandLine.AppendSwitchIfNotNull("/debug:", this.DebugType);
            //        commandLine.AppendPlusOrMinusSwitch("/delaysign", base.Bag, "DelaySign");
            //        commandLine.AppendSwitchWithInteger("/filealign:", base.Bag, "FileAlignment");
            //        commandLine.AppendSwitchIfNotNull("/keycontainer:", this.KeyContainer);
            //        commandLine.AppendSwitchIfNotNull("/keyfile:", this.KeyFile);
            //        commandLine.AppendSwitchIfNotNull("/linkresource:", this.LinkResources, new string[]
            //{
            //    "LogicalName",
            //    "Access"
            //});
            //        commandLine.AppendWhenTrue("/nologo", base.Bag, "NoLogo");
            //        commandLine.AppendWhenTrue("/nowin32manifest", base.Bag, "NoWin32Manifest");
            //        commandLine.AppendPlusOrMinusSwitch("/optimize", base.Bag, "Optimize");
            args.Output = OutputAssembly.ItemSpec;
            //commandLine.AppendSwitchIfNotNull("/out:", this.OutputAssembly);
            //        commandLine.AppendSwitchIfNotNull("/subsystemversion:", this.SubsystemVersion);
            //        commandLine.AppendSwitchIfNotNull("/resource:", this.Resources, new string[]
            //{
            //    "LogicalName",
            //    "Access"
            //});
            args.Target = TargetType;

            //commandLine.AppendSwitchIfNotNull("/target:", this.TargetType);
            //commandLine.AppendPlusOrMinusSwitch("/warnaserror", base.Bag, "TreatWarningsAsErrors");
            //commandLine.AppendWhenTrue("/utf8output", base.Bag, "Utf8Output");
            //commandLine.AppendSwitchIfNotNull("/win32icon:", this.Win32Icon);
            //commandLine.AppendSwitchIfNotNull("/win32manifest:", this.Win32Manifest);
            args.Files.AddRange(Sources.Select(t => t.ItemSpec).ToList());
            //commandLine.AppendFileNamesIfNotNull(this.Sources, " ");
        }
        protected void AddResponseFileCommands2(CompilerToolArgs args)
        {
            if (OutputGeneratedFile != null && !String.IsNullOrEmpty(OutputGeneratedFile.ItemSpec))
                args.OutputGeneratedFile = OutputGeneratedFile.ItemSpec;
            args.define = this.GetDefineConstantsSwitch(base.DefineConstants);
            this.AddReferencesToCommandLine2(args);
            BaseAddResponseFileCommands(args);
            if (ResponseFiles != null)
            {
                foreach (ITaskItem item in ResponseFiles)
                {
                    throw new Exception();
                    //commandLine.AppendSwitchIfNotNull("@", item.ItemSpec);
                }
            }
            if (ContentFiles != null)
            {
                foreach (var file in ContentFiles)
                {
                    args.ContentFiles.Add(file.ItemSpec);
                }
            }
            if (NoneFiles != null)
            {
                foreach (var file in NoneFiles)
                {
                    args.NoneFiles.Add(file.ItemSpec);
                }
            }
            if (SkcPlugins != null)
            {
                foreach (var file in SkcPlugins)
                {
                    args.Plugins.Add(file.ItemSpec);
                }
            }
            if (SkcRebuild)
                args.rebuild = true;
            if (UseBuildService)
            {
                args.CurrentDirectory = Directory.GetCurrentDirectory();
            }
            args.TargetFrameworkVersion = TargetFrameworkVersion;
        }
        private void AddReferencesToCommandLine2(CompilerToolArgs args)
        {
            if ((base.References != null) && (base.References.Length != 0))
            {
                foreach (ITaskItem item in base.References)
                {
                    string metadata = item.GetMetadata("Aliases");
                    if ((metadata == null) || (metadata.Length == 0))
                    {
                        args.References.Add(item.ItemSpec);
                    }
                    else
                    {
                        foreach (string str2 in metadata.Split(new char[] { ',' }))
                        {
                            string str3 = str2.Trim();
                            if (str2.Length != 0)
                            {
                                if (str3.IndexOfAny(new char[] { ',', ' ', ';', '"' }) != -1)
                                {
                                    throw new ArgumentException("Csc.AssemblyAliasContainsIllegalCharacters" + item.ItemSpec + str3);
                                }
                                if (string.Compare("global", str3, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    args.References.Add(item.ItemSpec);
                                }
                                else
                                {
                                    throw new NotImplementedException("TODO: Implement");
                                    //commandLine.AppendSwitchAliased("/reference:", str3, item.ItemSpec);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal static class TokenChar
    {
        // Methods
        internal static bool IsCombining(char c)
        {
            UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
            if ((unicodeCategory != UnicodeCategory.NonSpacingMark) && (unicodeCategory != UnicodeCategory.SpacingCombiningMark))
            {
                return false;
            }
            return true;
        }

        internal static bool IsConnecting(char c)
        {
            return (char.GetUnicodeCategory(c) == UnicodeCategory.ConnectorPunctuation);
        }

        internal static bool IsDecimalDigit(char c)
        {
            return (char.GetUnicodeCategory(c) == UnicodeCategory.DecimalDigitNumber);
        }

        internal static bool IsFormatting(char c)
        {
            return (char.GetUnicodeCategory(c) == UnicodeCategory.Format);
        }

        internal static bool IsHexDigit(char c)
        {
            if ((((c < '0') || (c > '9')) && ((c < 'A') || (c > 'F'))) && ((c < 'a') || (c > 'f')))
            {
                return false;
            }
            return true;
        }

        internal static bool IsLetter(char c)
        {
            UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
            if ((((unicodeCategory != UnicodeCategory.UppercaseLetter) && (unicodeCategory != UnicodeCategory.LowercaseLetter)) && ((unicodeCategory != UnicodeCategory.TitlecaseLetter) && (unicodeCategory != UnicodeCategory.ModifierLetter))) && ((unicodeCategory != UnicodeCategory.OtherLetter) && (unicodeCategory != UnicodeCategory.LetterNumber)))
            {
                return false;
            }
            return true;
        }

        internal static bool IsNewLine(char c)
        {
            if (((c != '\r') && (c != '\n')) && (c != '\u2028'))
            {
                return (c == '\u2029');
            }
            return true;
        }

        internal static bool IsOctalDigit(char c)
        {
            return ((c >= '0') && (c <= '7'));
        }
    }




}
