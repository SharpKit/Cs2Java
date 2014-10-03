using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Compiler
{
    class CompilerService
    {
        public CompileResponse Compile(CompileRequest args)
        {
            var skc = new CompilerTool { Args = args.Args, CommandLineArguments = new string[] { args.CommandLineArgs } };
            var x = skc.Run();
            var xx = new CompileResponse { Output = skc.Log.Console.Items.ToList(), ExitCode = x };
            return xx;

        }
    }

    [DataContract]
    class CompileRequest
    {
        [DataMember]
        public string CommandLineArgs { get; set; }
        [DataMember]
        public CompilerToolArgs Args { get; set; }
    }
    [DataContract]
    class CompileResponse
    {
        [DataMember]
        public List<string> Output { get; set; }
        [DataMember]
        public int ExitCode { get; set; }
    }

}
