using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace JSharp.Compiler
{
    [DataContract]
    public class CompilerToolArgs
    {
        public CompilerToolArgs()
        {
            Files = new List<string>();
            References = new List<string>();
            ContentFiles = new List<string>();
            NoneFiles = new List<string>();
            ResourceFiles = new List<string>();
        }
        [DataMember]
        public List<string> Files { get; private set; }


        [DataMember]
        public bool Help { get; set; }

        /// <summary>
        /// designates the current directory that all paths are relative to
        /// </summary>
        [DataMember]
        public string CurrentDirectory { get; set; }

        [DataMember]
        public string Target { get; set; }

        [DataMember]
        public string Output { get; set; }

        [DataMember]
        public List<string> References { get; private set; }

        [DataMember]
        public List<string> Plugins { get; private set; }

        [DataMember]
        public List<string> ContentFiles { get; private set; }

        [DataMember]
        public List<string> ResourceFiles { get; private set; }

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

        [DataMember]
        public string errorreport { get; set; }

        [DataMember]
        public int warn { get; set; }

        [DataMember]
        public string nowarn { get; set; }

        [DataMember]
        public string define { get; set; }

        [DataMember]
        public string debugLevel { get; set; }

        [DataMember]
        public bool? debug { get; set; }

        [DataMember]
        public bool? optimize { get; set; }

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
        [DataMember]
        public string AddBuildTarget { get; set; }

        [DataMember]
        public string TargetFrameworkVersion { get; set; }

        [DataMember]
        public bool CreateNativeImage { get; set; }

    }
}
