using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.TypeSystem
{
    public partial class CSharpAssembly : IAssembly
    {
        public object Tag { get; set; }
    }
}
namespace ICSharpCode.NRefactory.CSharp.Resolver
{
    partial class CSharpResolver
    {
        static ResolveResult DynamicResult { get { return new ResolveResult(SpecialType.Dynamic); } }

    }
    partial class CSharpOperators
    {
        partial class OperatorMethod
        {
            public object Tag { get; set; }
        }
    }
}
namespace ICSharpCode.NRefactory.CSharp
{
    public interface ICSharpResolveResultVisitor<out R> : IResolveResultVisitor<R>
    {
        R VisitCSharpInvocationResolveResult(CSharpInvocationResolveResult res);
        R VisitLambdaResolveResult(LambdaResolveResult res);
        R VisitMethodGroupResolveResult(MethodGroupResolveResult res);


        R VisitDynamicInvocationResolveResult(DynamicInvocationResolveResult res);
        R VisitDynamicMemberResolveResult(DynamicMemberResolveResult res);
    }

    partial class ReducedExtensionMethod
    {
        public object Tag { get; set; }
    }
}
namespace ICSharpCode.NRefactory.CSharp.Resolver
{
    partial class CSharpInvocationResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitCSharpInvocationResolveResult(this); }
    }
    partial class LambdaResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitLambdaResolveResult(this); }
    }
    partial class MethodGroupResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitMethodGroupResolveResult(this); }
    }

    partial class DynamicInvocationResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitDynamicInvocationResolveResult(this); }
    }
    partial class DynamicMemberResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitDynamicMemberResolveResult(this); }
    }

}
