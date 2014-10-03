using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSharp.CSharp;

namespace JSharp.JsonToCSharp
{
    class JavaAssemblyContext
    {
        public JavaAssemblyContext()
        {
            JavaCoreAssembly = new JavaCoreAssembly();

            Context = new AssemblyContext
            {
                Assemblies = { JavaCoreAssembly },
                ClassMappings = new Dictionary<string, string>
                {
                    //                    {"boolean", "bool"},
                },
            };
            WildcardClass = Context.GetClass("object");
            ObjectClass = Context.GetClass("java.lang.Q");
            VoidClass = Context.GetClass("void");

            EnumeratorIface = new Class
            {
                FullName = "System.Collections.Generic.IEnumerator",
                GenericArguments = { new Class { Name = "T", IsGenericTypeArgument = true } },
            };
            EnumerableIface = new Class
            {
                FullName = "System.Collections.Generic.IEnumerable",
                GenericArguments = { new Class { Name = "T", IsGenericTypeArgument = true } },
            };
            EnumerableIface.Members.Add(new Method { Name = "GetEnumerator", Type = Context.MakeGenericClass(EnumeratorIface, EnumerableIface.GenericArguments) });
            JavaCoreAssembly.Classes.Add(EnumerableIface);
            JavaCoreAssembly.Classes.Add(EnumeratorIface);
        }


        public JavaCoreAssembly JavaCoreAssembly { get; set; }
        public AssemblyContext Context { get; set; }
        public Class VoidClass { get; set; }
        public Class WildcardClass { get; set; }
        public Class ObjectClass { get; set; }
        public Class JavaLangObjectClass { get; set; }
        public List<Assembly> Assemblies { get { return Context.Assemblies; } }


        public Class EnumeratorIface { get; set; }

        public Class EnumerableIface { get; set; }
    }
}
