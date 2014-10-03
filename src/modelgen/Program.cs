using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemUtils.Tools;

namespace modelgen
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var x = T4ModelGenTool.ExtractDataFromTextTemplate(@"C:\Projects\c2j\src\cs2java\Java\Ast\JModel2.tt");
                var xx = new T4ModelGenTool().Debug(@"C:\Projects\c2j\src\cs2java\Java\Ast\JModel2.tt", "JSharp.Java.Ast");
                return 0;
                Console.WriteLine("modelgen "+args.NotNull().StringConcat(" "));
                var tool = ToolArgsParser.Parse<ModelGenTool>(args);
                if (tool.Help)
                {
                    Console.WriteLine(ToolArgsGenerator.GenerateHelp(tool));
                    return 0;
                }
                var gen = new ModelGen
                {
                    SourceFilename = tool.SourceFilename,
                    GenerateEnum = tool.GenerateEnum.GetValueOrDefault(true),
                    GenerateLinq = tool.GenerateLinq.GetValueOrDefault(true),
                    GenerateProperties = tool.GenerateProperties.GetValueOrDefault(true),
                    RootNodeTypeName = tool.RootNodeTypeName,
                    Namespace = tool.Namespace,
                };
                if(tool.Using!=null)
                    gen.Usings.AddRange(tool.Using);
                gen.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
            return 0;
            //new JModelGenerator
            //{
            //    SourceFilename = @"..\..\CsModel.txt",
            //    BaseNodeTypeName = "Element",
            //    GenerateLinq = true,
            //    Namespace = "JSharp.CSharp",
            //}.Run();
            //return;
            //new JModelGenerator
            //{
            //    SourceFilename = @"..\..\JModel.txt",
            //    GenerateEnum = true,
            //    GenerateLinq = true,
            //    GenerateProperties = true,
            //    BaseNodeTypeName = "JNode",
            //    Namespace = "JSharp.Java.Ast"
            //}.Run();

        }
    }
}
