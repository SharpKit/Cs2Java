using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple=true)]
    public class JTypeAttribute : Attribute
    {
        public bool NativeOperatorOverloads { get; set; }
        public Type TargetType { get; set; }
        public string Name { get; set; }
        public bool Export { get; set; }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class JMethodAttribute : Attribute
    {
        public Type TargetType { get; set; }
        public string TargetMethod { get; set; }
        public string Name { get; set; }
        public bool Export { get; set; }
        public bool OmitCalls { get; set; }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class JPropertyAttribute : Attribute
    {
        public Type TargetType { get; set; }
        public string TargetProperty { get; set; }
        public string Name { get; set; }
        public bool Export { get; set; }
        public bool NativeField { get; set; }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class JExportAttribute : Attribute
    {
        public string OutputDir { get; set; }
    }

    public class JFieldAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Export { get; set; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class FinalAttribute : Attribute
    {
    }
}
