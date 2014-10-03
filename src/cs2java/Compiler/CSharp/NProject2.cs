using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using JSharp.Compiler;
using System.Collections.Concurrent;
using JSharp.Utils;
using JSharp.Utils.Misc;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    class NProject2 : NProject
    {
        public NProject2()
        {
            Parallel = CollectionExtensions.Parallel;
        }
        public CompilerTool Compiler { get; set; }

        protected override void ParseCsFiles()
        {
            base.ParseCsFiles();
            if (!CSharpParser.HasErrors)
                return;
            foreach (var error in CSharpParser.ErrorsAndWarnings)
            {
                var item = new CompilerLogItem
                {
                    ProjectRelativeFilename = error.Region.FileName,
                    Line = error.Region.BeginLine,
                    Column = error.Region.BeginColumn,
                    Text = error.Message,
                    Type = CompilerLogItemType.Error,
                };
                if (error.ErrorType == ErrorType.Warning)
                    item.Type = CompilerLogItemType.Warning;
                Compiler.Log.Log(item);

            }
        }



        protected override void WriteLine(object obj)
        {
            Compiler.Log.WriteLine("{0:HH:mm:ss.fff}: {1}", DateTime.Now, obj);
        }
        protected override void FormatLine(string format, params object[] args)
        {
            WriteLine(String.Format(format, args));
        }


    }

}
