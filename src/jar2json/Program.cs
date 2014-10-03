using java.lang;
using java.util;
using System.Linq;
using JSharp;

namespace JSharpTest
{
    class Program
    {
        [JMethod(Export=false)]
        public static void Main(string[] args)
        {
        }
        public static void main(String[] args)
        {
            //String classpath = Sys.getProperty("java.class.path");
            //Sys.@out.println(classpath);
            Sys.@out.println("Asdasd");
            new JarExporter().run(args);
        }
    }
}
