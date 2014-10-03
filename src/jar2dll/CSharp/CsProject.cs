using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utils.CSharp
{
    class CsProjectHelper
    {
        public static void SaveCsProject(List<string> compileItems, string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if(!dir.EndsWith("\\"))
                dir +="\\";
            var files = compileItems.Select(t=>t.Replace(dir, "")).ToList();
            var template= File.ReadAllText("CSharp\\CsProjTemplate.txt");
            var sb = new StringBuilder();
            foreach (var item in files)
            {
                sb.AppendLine(String.Format("<Compile Include=\"{0}\" />", item));
            }
            var ss = template.Replace("<!--PLACEHOLDER-->", sb.ToString());
            File.WriteAllText(filename, ss);
        }
    }
}
