using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JSharp.Compiler;

namespace JSharp.Java.Ast
{
    class JFile
    {
        public JFile()
        {
            Units = new List<JCompilationUnit>();
        }
        public string Filename { get; set; }
        public List<JCompilationUnit> Units { get; set; }

        public void CompareAndSave()
        {
            var tmpFile = Filename + ".tmp";
            SaveAs(tmpFile);
            FileUtils.CompareAndSaveFile(Filename, tmpFile);


        }

        public void SaveAs(string filename)
        {
            var tmpFile = filename;
            using (var writer = JWriter.Create(tmpFile, false))
            {
                try
                {
                    if (CompilerConfig.Current.EnableLogging)
                    {
                        writer.Visiting += node =>
                            {
                                Compiler.Log.Debug(String.Format("JsWriter: Visit JsNode: {0}: [{1}, {2}]", filename, writer.CurrentLine, writer.CurrentColumn));
                            };
                    }
                    writer.VisitEach(Units);
                }
                catch (Exception e)
                {
                    Compiler.Log.Log(new CompilerLogItem { Type = CompilerLogItemType.Error, ProjectRelativeFilename = tmpFile, Text = e.Message });
                    throw e;
                }
            }
        }
        public CompilerTool Compiler { get; set; }

    }

    partial class JsExternalFileUnit : JUnit
    {
        public string Filename { get; set; }
    }
}
