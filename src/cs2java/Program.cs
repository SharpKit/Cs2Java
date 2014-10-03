using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using System.IO;
using System.CodeDom.Compiler;
using JSharp.Java;
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using System.Globalization;

namespace JSharp.Compiler
{
    class Program
    {
        static int Main(string[] args)
        {

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            CollectionExtensions.Parallel = ConfigurationManager.AppSettings["Parallel"] == "true";
            CollectionExtensions.ParallelPreAction = () => Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            //Console.AutoFlush = true;
            Console.WriteLine("Parallel=" + CollectionExtensions.Parallel);
            var skc = new CompilerTool { CommandLineArguments = args };
#if DEBUG
            skc.Debug = true;
#endif
            var res = skc.Run();
            stopwatch.Stop();
            //Console.FormatLine("Total: {0}ms", stopwatch.ElapsedMilliseconds);
            //Console.Flush();
            return res;

        }

    }



}
