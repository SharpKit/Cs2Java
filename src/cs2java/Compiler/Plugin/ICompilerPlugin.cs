using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using JSharp.Java.Ast;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp;

namespace JSharp.Compiler
{
    public interface ICompilerPlugin
    {
        void Init(ICompiler compiler);
    }



}
