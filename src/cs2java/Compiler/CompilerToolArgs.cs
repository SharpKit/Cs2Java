using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SystemUtils.Tools;
using System.Runtime.Serialization;

namespace JSharp.Compiler
{
    [DataContract]
    class CompilerToolArgs
    {
        public CompilerToolArgs()
        {
            Files = new List<string>();
            References = new List<string>();
            ContentFiles = new List<string>();
            NoneFiles = new List<string>();
            ResourceFiles = new List<string>();
        }
        public static CompilerToolArgs Parse(string[] args)
        {
            var args2 = new CompilerToolArgs();
            ToolArgsParser.ParseInto(args2, args);
            return args2;
        }
        public static void GenerateHelp(TextWriter writer)
        {
            ToolArgsGenerator.GenerateHelp(typeof(CompilerToolArgs), writer);

        }
        [DataMember]
        [ToolArgDefault]
        [ToolArgTrimValueChars(new char[] { '\"' })]
        public List<string> Files { get; private set; }


        [DataMember]
        [ToolArgSyntax("/?")]
        public bool Help { get; set; }

        /// <summary>
        /// designates the current directory that all paths are relative to
        /// </summary>
        [ToolArgSyntax("/dir:{0}")]
        [DataMember]
        public string CurrentDirectory { get; set; }

        [ToolArgSyntax("/target:{0}")]
        [DataMember]
        public string Target { get; set; }

        [ToolArgSyntax("/out:{0}")]
        [DataMember]
        public string Output { get; set; }

        [ToolArgSyntax("/reference:{0}")]
        [ToolArgTrimValueChars(new char[] { '\"' })]
        [DataMember]
        public List<string> References { get; private set; }

        [ToolArgSyntax("/plugin:{0}")]
        [ToolArgTrimValueChars(new char[] { '\"' })]
        [DataMember]
        public List<string> Plugins { get; private set; }

        [ToolArgSyntax("/contentfile:{0}")]
        [ToolArgTrimValueChars(new char[] { '\"' })]
        [DataMember]
        public List<string> ContentFiles { get; private set; }

        [ToolArgSyntax("/resource:{0}")]
        [ToolArgTrimValueChars(new char[] { '\"' })]
        [DataMember]
        public List<string> ResourceFiles { get; private set; }

        [ToolArgSyntax("/nonefile:{0}")]
        [ToolArgTrimValueChars(new char[] { '\"' })]
        [DataMember]
        public List<string> NoneFiles { get; private set; }

        //[CommandLineArgumentSyntax]//("/target:{0}")]
        [DataMember]
        public bool why { get; set; }

        [DataMember]
        public bool? rebuild { get; set; }
        [DataMember]
        public bool? Enabled { get; set; }
        [DataMember]
        public bool? ExportToCSharp { get; set; }
        [DataMember]
        public bool? DebuggerBreak { get; set; }
        [DataMember]
        public bool? noconfig { get; set; }
        [DataMember]
        public bool? UseLineDirectives { get; set; }

        [ToolArgSyntax("/errorreport:{0}")]
        [DataMember]
        public string errorreport { get; set; }

        [ToolArgSyntax("/warn:{0}")]
        [DataMember]
        public int warn { get; set; }

        [ToolArgSyntax("/nowarn:{0}")]
        [DataMember]
        public string nowarn { get; set; }

        [ToolArgSyntax("/define:{0}")]
        [DataMember]
        public string define { get; set; }

        [ToolArgSyntax("/debug:{0}")]
        [DataMember]
        public string debugLevel { get; set; }

        [DataMember]
        public bool? debug { get; set; }

        [DataMember]
        public bool? optimize { get; set; }

        [ToolArgSyntax("/filealign:{0}")]
        [DataMember]
        public int filealign { get; set; }

        private string _AssemblyName;
        public string AssemblyName
        {
            get
            {
                if (_AssemblyName == null)
                {
                    _AssemblyName = Path.GetFileNameWithoutExtension(Output);
                }
                return _AssemblyName;
            }
        }
        [DataMember]
        public string ManifestFile { get; set; }

        [DataMember]
        public string CodeAnalysisFile { get; set; }

        [DataMember]
        public string SecurityAnalysisFile { get; set; }

        [DataMember]
        public string OutputGeneratedJsFile { get; set; }

        [ToolArgSyntax("/outputgeneratedfile:{0}")]
        [DataMember]
        public string OutputGeneratedFile { get; set; }

        [DataMember]
        public string OutputGeneratedDir { get; set; }

        [DataMember]
        public bool CheckForNewVersion { get; set; }

        [DataMember]
        public bool Service { get; set; }
        /// <summary>
        /// /addbuildtarget:"pathToCsprojFile"
        /// /addbuildtarget:"pathToCsprojFile";nuget
        /// </summary>
        [ToolArgSyntax("/addbuildtarget:{0}")]
        [ToolArgTrimValueChars(new char[] { '\"' })]
        [DataMember]
        public string AddBuildTarget { get; set; }

        [DataMember]
        public string TargetFrameworkVersion { get; set; }

        [ToolArgSyntax("/ngen")]
        [DataMember]
        public bool CreateNativeImage { get; set; }

    }
}
