using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JSharp.Java.Ast;
using JSharp.Compiler.SourceMapping;

namespace JSharp.Compiler
{
    class JFileWrapper
    {
        public override string ToString()
        {
            if (JsFile != null && JsFile.Filename.IsNotNullOrEmpty())
                return JsFile.Filename;
            return base.ToString();
        }
        public bool GenerateSourceMap { get; set; }
        public JFile JsFile { get; set; }
        public string TempFilename { get; set; }
        public CompilerTool Compiler { get; set; }
        public void Save()
        {
            var jsFile = JsFile;
            Compiler.Log.WriteLine("    {0}", jsFile.Filename);
            var ext = Path.GetExtension(jsFile.Filename).ToLower();
            if (TempFilename.IsNullOrEmpty())
                TempFilename = jsFile.Filename + ".tmp";
            var dir = Path.GetDirectoryName(TempFilename);
            if (dir.IsNotNullOrEmpty() && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            jsFile.SaveAs(TempFilename);
            if (GenerateSourceMap)
            {
                var smg = new SourceMappingGenerator { Compiler = Compiler };
                smg.TryGenerateAndAddMappingDirective(this);
            }
            FileUtils.CompareAndSaveFile(jsFile.Filename, TempFilename);
        }
    }
}
