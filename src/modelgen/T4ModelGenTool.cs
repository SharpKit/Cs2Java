using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace modelgen
{
    public class T4ModelGenTool
    {
        public T4ModelGenTool()
        {
            Generator = new ModelGen
            {
                GenerateLinq = true,
                GenerateProperties = true,
            };
        }
        public static string ExtractDataFromTextTemplate(string filename)
        {
            var s = File.ReadAllText(filename);
            s = s.SubstringBetween("/*", "*/").Trim();
            return s;
        }
        public void Run(object tt, object host)
        {
            var tt2 = new ReflectionTextTemplating(tt);
            var host2 = new ReflectionTextTemplatingHost(host);
            if (Generator.SourceText == null)
                Generator.SourceText = ExtractDataFromTextTemplate(host2.TemplateFile);
            if (Generator.Namespace == null)
                Generator.Namespace = (string)CallContext.LogicalGetData("NamespaceHint");
            Generator.Run();
            foreach (var line in Generator.Output.Lines())
            {
                tt2.WriteLine(line);
            }
        }
        public string Debug(string templateFile, string ns)
        {
            if (Generator.SourceText == null)
                Generator.SourceText = ExtractDataFromTextTemplate(templateFile);
            if (Generator.Namespace == null)
                Generator.Namespace = ns;
            Generator.Run();
            return Generator.Output;
            //foreach (var line in Generator.Output.Lines())
            //{
            //    tt2.WriteLine(line);
            //}
        }



        public ModelGen Generator { get; set; }
    }
}
