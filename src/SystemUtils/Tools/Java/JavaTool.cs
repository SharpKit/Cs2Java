using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemUtils.Tools.Java
{
    public class JavaTool
    {
        public JavaTool()
        {
            ClassPath = new List<string>();
        }
        public static JavaTool AutoDetect(string dir)
        {
            var binDir = Path.Combine(dir, "bin");
            var mainFile = Directory.GetFiles(binDir, "Program.class", SearchOption.AllDirectories)[0];
            var mainClass = mainFile.Replace(".class", "");
            mainClass = mainClass.Replace(binDir, "");

            if (mainClass.StartsWith("\\"))
                mainClass = mainClass.Substring(1);
            mainClass = mainClass.Replace("\\", ".");

            var libDir = Path.Combine(dir, "lib");

            var java = new JavaTool
            {
                ClassPath = { binDir, },
                MainClass = mainClass,
            };
            if (Directory.Exists(libDir))
                java.AddAllJarsInDirToClassPath(libDir);
            return java;
        }
        public void AddAllJarsInDirToClassPath(string dir)
        {
            ClassPath.AddRange(Directory.GetFiles(dir, "*.jar"));
        }
        public List<string> ClassPath { get; set; }
        public string MainClass { get; set; }
        public string Arguments { get; set; }

        public string MaxPermSize { get; set; }
        public Process Process { get; set; }
        public void Run()
        {
            var sb = new StringBuilder();
            if(MaxPermSize!=null)
                sb.Append("-XX:MaxPermSize="+MaxPermSize+" ");
            sb.Append("-classpath ");
            sb.Append(String.Join(";", ClassPath.ToArray()));
            sb.Append(" ");
            sb.Append(MainClass);
            sb.Append(" ");
            sb.Append(Arguments);
            var args = sb.ToString();
            Process = new Process { StartInfo = new ProcessStartInfo { FileName = @"C:\Program Files\Java\jdk1.7.0_25\bin\javaw.exe", Arguments = args, UseShellExecute = false } };
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
            Process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            Process.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);
            Process.WaitForExit();
        }

    }
}