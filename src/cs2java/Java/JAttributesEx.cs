using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;

namespace JSharp
{
    partial class JTypeAttribute : Attribute, ISupportJSharpVersion
    {
        public IType TargetType { get; set; }
    }
    partial class JMethodAttribute : Attribute, ISupportJSharpVersion
    {
        public IType TargetType { get; set; }
    }
    partial class JPropertyAttribute : Attribute
    {
        public IType TargetType { get; set; }
    }
    partial class JEnumAttribute : Attribute
    {
        public IType TargetType { get; set; }
    }

    partial class JEmbeddedResourceAttribute : ISupportSourceAttribute
    {
        public IAttribute SourceAttribute { get; set; }
    }

    interface ISupportSourceAttribute
    {
        IAttribute SourceAttribute { get; set; }

    }

    interface ISupportJSharpVersion
    {
        string JSharpVersion { get; set; }

    }

}
