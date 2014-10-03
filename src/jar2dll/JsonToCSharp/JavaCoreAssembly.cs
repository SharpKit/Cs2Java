using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSharp.CSharp;

namespace JSharp.JsonToCSharp
{
    class JavaCoreAssembly : Assembly
    {
        public JavaCoreAssembly()
        {
            Classes.AddRange
            (
                new Class[]
                {
                    new Class{Name="void"},
                    new Class{Name="int"},
                    new Class{Name="long"},
                    new Class{Name="byte"},
                    new Class{Name="short"},
                    new Class{Name="int"},
                    new Class{Name="float"},
                    new Class{Name="double"},
                    new Class{Name="char"},
                    new Class{Name="boolean"},
                    new Class{Name="object"},
                    new Class{FullName="java.lang.Q"},
                    new Class{FullName="System.Exception"},
                    
                }
            );

        }
    }
}
