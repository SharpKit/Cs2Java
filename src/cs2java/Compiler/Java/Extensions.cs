using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using JSharp.Java.Ast;

namespace JSharp.Compiler
{
    static class JsNodeExtensions2
    {
        public static JsNodeEx Ex(this JNode node, bool create = false)
        {
            var md = node.Metadata as JsNodeEx;
            if (md == null && create)
            {
                md = new JsNodeEx();
                node.Metadata = md;
            }
            return md;
        }
    }
    class JsNodeEx
    {
        public string NamespaceVerification { get; set; }
        public AstNode AstNode { get; set; }
    }
}
