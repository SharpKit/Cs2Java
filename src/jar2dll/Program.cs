using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using jrun;
using JSharp.JsonToCSharp;

namespace JSharp.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseDir = @"C:\Projects\c2j\jlib\";
            Rt = new LibInfo
            {
                JarFilename = Path.Combine(baseDir, @"jdk1.7.0_25\rt.jar"),
            };

            Libs = new List<LibInfo> { Rt };
            Libs.AddRange(Directory.GetFiles(baseDir + "jackson", "*.jar").Where(t => !t.Contains("-sources")).Select(ToLibInfo));
            Libs.Add(new LibInfo{JarFilename=Path.Combine(baseDir, @"hadoop-core\hadoop-core-1.0.0.jar")});

            foreach (var lib in Libs)
            {
                lib.JsonFilename = Path.ChangeExtension(lib.JarFilename, ".json");
                lib.CSharpDir = Path.Combine(Path.GetDirectoryName(lib.JarFilename), Path.GetFileNameWithoutExtension(lib.JarFilename));
            }

            //foreach (var lib in Libs)
            //{
            //    if (!File.Exists(lib.JsonFilename) || new FileInfo(lib.JsonFilename).Length == 0)
            //    {
            //        JarToJson(lib);
            //    }
            //}
            //JarToJson(GetLib("hadoop-core-1.0.0.jar"));
            //return;
            //return;
            JsonToCSharp(new List<LibInfo>
            {
                GetLib("hadoop-core-1.0.0.jar"),
                //GetLib("jackson-asl-0.9.5.jar"), 
//                  GetLib("jackson-mapper-asl-1.9.11") ,
            }, new List<LibInfo> 
            { 
                  GetLib("rt.jar"), 
  //                GetLib("rt.jar"), 
    //            GetLib("jackson-core-asl-1.9.11.jar"), 
            });

            //JarToJson(Rt);
            //return;
            //JsonToCSharp(Rt);
            //JsonToCSharp(Jackson, Rt);
        }

        static LibInfo GetLib(string name)
        {
            return Libs.Where(t => t.JarFilename.Contains(name)).Single();
        }


        static LibInfo ToLibInfo(string jarFilename)
        {
            var lib = new LibInfo
            {
                JarFilename = jarFilename,
            };
            lib.JsonFilename = Path.ChangeExtension(lib.JarFilename, ".json");
            lib.CSharpDir = Path.Combine(Path.GetDirectoryName(lib.JarFilename), Path.GetFileNameWithoutExtension(lib.JarFilename));
            return lib;
        }

        private static string Quote(string p)
        {
            return "\"" + p + "\"";
        }
        private static void JarToJson(string jarFilename, string jsonFilename)
        {
            var java = JavaTool.AutoDetect(@"C:\Projects\workspace\jar2json");//C:\Projects\JSharpKit\JarExport");
            java.MaxPermSize = "512M";
            java.ClassPath.Add(@"C:\Projects\workspace\jcorlib\bin\");
            java.ClassPath.AddRange(new string[]{
@"C:\Users\dkhen\.m2\repository\org\apache\hadoop\hadoop-core\1.0.0\hadoop-core-1.0.0.jar",
@"C:\Users\dkhen\.m2\repository\commons-cli\commons-cli\1.2\commons-cli-1.2.jar",
@"C:\Users\dkhen\.m2\repository\xmlenc\xmlenc\0.52\xmlenc-0.52.jar",
@"C:\Users\dkhen\.m2\repository\commons-httpclient\commons-httpclient\3.0.1\commons-httpclient-3.0.1.jar",
@"C:\Users\dkhen\.m2\repository\junit\junit\3.8.1\junit-3.8.1.jar",
@"C:\Users\dkhen\.m2\repository\commons-logging\commons-logging\1.0.3\commons-logging-1.0.3.jar",
@"C:\Users\dkhen\.m2\repository\commons-codec\commons-codec\1.4\commons-codec-1.4.jar",
@"C:\Users\dkhen\.m2\repository\org\apache\commons\commons-math\2.1\commons-math-2.1.jar",
@"C:\Users\dkhen\.m2\repository\commons-configuration\commons-configuration\1.6\commons-configuration-1.6.jar",
@"C:\Users\dkhen\.m2\repository\commons-collections\commons-collections\3.2.1\commons-collections-3.2.1.jar",
@"C:\Users\dkhen\.m2\repository\commons-lang\commons-lang\2.4\commons-lang-2.4.jar",
@"C:\Users\dkhen\.m2\repository\commons-digester\commons-digester\1.8\commons-digester-1.8.jar",
@"C:\Users\dkhen\.m2\repository\commons-beanutils\commons-beanutils\1.7.0\commons-beanutils-1.7.0.jar",
@"C:\Users\dkhen\.m2\repository\commons-beanutils\commons-beanutils-core\1.8.0\commons-beanutils-core-1.8.0.jar",
@"C:\Users\dkhen\.m2\repository\commons-net\commons-net\1.4.1\commons-net-1.4.1.jar",
@"C:\Users\dkhen\.m2\repository\org\mortbay\jetty\jetty\6.1.26\jetty-6.1.26.jar",
@"C:\Users\dkhen\.m2\repository\org\mortbay\jetty\servlet-api\2.5-20081211\servlet-api-2.5-20081211.jar",
@"C:\Users\dkhen\.m2\repository\org\mortbay\jetty\jetty-util\6.1.26\jetty-util-6.1.26.jar",
@"C:\Users\dkhen\.m2\repository\tomcat\jasper-runtime\5.5.12\jasper-runtime-5.5.12.jar",
@"C:\Users\dkhen\.m2\repository\tomcat\jasper-compiler\5.5.12\jasper-compiler-5.5.12.jar",
@"C:\Users\dkhen\.m2\repository\org\mortbay\jetty\jsp-api-2.1\6.1.14\jsp-api-2.1-6.1.14.jar",
@"C:\Users\dkhen\.m2\repository\org\mortbay\jetty\servlet-api-2.5\6.1.14\servlet-api-2.5-6.1.14.jar",
@"C:\Users\dkhen\.m2\repository\org\mortbay\jetty\jsp-2.1\6.1.14\jsp-2.1-6.1.14.jar",
@"C:\Users\dkhen\.m2\repository\ant\ant\1.6.5\ant-1.6.5.jar",
@"C:\Users\dkhen\.m2\repository\commons-el\commons-el\1.0\commons-el-1.0.jar",
@"C:\Users\dkhen\.m2\repository\net\java\dev\jets3t\jets3t\0.7.1\jets3t-0.7.1.jar",
@"C:\Users\dkhen\.m2\repository\net\sf\kosmosfs\kfs\0.3\kfs-0.3.jar",
@"C:\Users\dkhen\.m2\repository\hsqldb\hsqldb\1.8.0.10\hsqldb-1.8.0.10.jar",
@"C:\Users\dkhen\.m2\repository\oro\oro\2.0.8\oro-2.0.8.jar",
@"C:\Users\dkhen\.m2\repository\org\eclipse\jdt\core\3.1.1\core-3.1.1.jar",
@"C:\Users\dkhen\.m2\repository\org\codehaus\jackson\jackson-mapper-asl\1.0.1\jackson-mapper-asl-1.0.1.jar",
@"C:\Users\dkhen\.m2\repository\org\codehaus\jackson\jackson-core-asl\1.0.1\jackson-core-asl-1.0.1.jar"
            });
            //java.AddAllJarsInDirToClassPath(@"C:\Projects\workspace\jar2json\lib");
            java.Arguments = Quote(jarFilename) + " " + Quote(jsonFilename);
            java.Run();
        }

        private static void JarToJson(LibInfo lib)
        {
            Console.WriteLine("{0} {1}", lib.JarFilename, lib.JsonFilename);
            JarToJson(lib.JarFilename, lib.JsonFilename);
        }

        static void JsonToCSharp(List<LibInfo> libs, List<LibInfo> deps)
        {
            var context = new JavaAssemblyContext();
            var libs2 = libs.Select(lib => new JsonToCSharpConverter
            {
                JarJsonFilename = lib.JsonFilename,
                OutputDir = lib.CSharpDir,
                Context = context,
            }).ToList();
            var deps2 = deps.Select(dep => new JsonToCSharpConverter
            {
                JarJsonFilename = dep.JsonFilename,
                OutputDir = dep.CSharpDir,
                Context = context,
            }).ToList();

            var all = deps2.Concat(libs2).ToList();
            all.ForEach(t => t.LoadConvert());
            //all.ForEach(t => t.RemoveMembersWithUnresolvedTypes());
            all.ForEach(t => t.FixAssembly());
            all.ForEach(t => t.FilterOutIfNeeded());
            libs2.ForEach(t => t.Export());
        }


        public static LibInfo Rt { get; set; }


        public static List<LibInfo> Libs { get; set; }
    }

    class LibInfo
    {
        public string JarFilename { get; set; }
        public string JsonFilename { get; set; }
        public string CSharpDir { get; set; }
    }
}
