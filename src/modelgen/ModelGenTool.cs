using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TextTemplating;
using SystemUtils.Tools;

namespace modelgen
{
    public class ModelGenTool
    {
        [ToolArgDefault]
        public string SourceFilename { get; set; }
        public string RootNodeTypeName { get; set; }
        public bool? GenerateLinq { get; set; }
        public bool? GenerateEnum { get; set; }
        public bool? GenerateProperties { get; set; }
        public string Namespace { get; set; }
        public List<string> Using { get; set; }
        public bool Help { get; set; }
        public void Execute()
        {
            new Tool
            {
                Filename = @"C:\Projects\c2j\src\modelgen\bin\Debug\modelgen.exe",
                Args = ToolArgsGenerator.Generate(this),
            }.Run();
        }
        public void Run()
        {
            Generator = new ModelGen
            {
                SourceText = SourceText,
                SourceFilename = SourceFilename,
                GenerateEnum = GenerateEnum.GetValueOrDefault(true),
                GenerateLinq = GenerateLinq.GetValueOrDefault(true),
                GenerateProperties = GenerateProperties.GetValueOrDefault(true),
                RootNodeTypeName = RootNodeTypeName,
                Namespace = Namespace,
            };
            if (OutputFilename == null)
                Generator.OutputFilename = SourceFilename.ToFsPath().ChangeExtension(".generated.cs");
            if (Using != null)
                Generator.Usings.AddRange(Using);
            Generator.Run();
        }
        internal ModelGen Generator;

        public string SourceText { get; set; }

        public string OutputFilename { get; set; }
    }



    static class Extensions2
    {
        public static T ReflectionGetPropertyValue<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetProperty(name).GetValue(obj, null);
        }
        public static T ReflectionInvoke<T>(this object obj, string name, params object[] args)
        {
            return (T)obj.GetType().GetMethods().Where(t => t.Name == name && t.GetParameters().Length == args.Length).First().Invoke(obj, args);
        }
        public static void ReflectionInvoke(this object obj, string name, params object[] args)
        {
            obj.ReflectionInvoke<object>(name, args);
        }
    }
    class ReflectionTextTemplating
    {
        public ReflectionTextTemplating(object obj)
        {
            Obj = obj;
        }
        object Obj;

        public void WriteLine(string p)
        {
            Obj.ReflectionInvoke("WriteLine", p);
        }

    }
    class ReflectionTextTemplatingHost
    {
        public ReflectionTextTemplatingHost(object obj)
        {
            Obj = obj;
        }
        object Obj;

        public string TemplateFile
        {
            get
            {
                return Obj.ReflectionGetPropertyValue<string>("TemplateFile");
            }
        }
    }


}
