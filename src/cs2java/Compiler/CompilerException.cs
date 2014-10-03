using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace JSharp.Compiler
{
    class CompilerException : Exception
    {
        public CompilerException(AstNode node, Exception innerException)
            : base(innerException.Message, innerException)
        {
            AstNode = node;
        }
        public CompilerException(IEntity me, AstNode node, string msg)
            : base(msg)
        {
            AstNode = node;
            Entity = me;
        }
        public CompilerException(string filename, int line, int col, string msg)
            : base(msg)
        {
            Filename = filename;
            Line = line;
            Column = col;
        }
        public CompilerException(IEntity me, string msg)
            : base(msg)
        {
            Entity = me;
        }
        public CompilerException(IAttribute att, string msg)
            : base(msg)
        {
            if (att == null)
                return;
            Filename = att.Region.FileName;
            Line = att.Region.BeginLine;
            Column = att.Region.BeginColumn;
        }
        public CompilerException(IEntity me, string msg, Exception innerException)
            : base(msg, innerException)
        {
            Entity = me;
        }
        public CompilerException(AstNode node, string msg)
            : base(msg)
        {
            AstNode = node;
        }
        public string Filename { get; set; }
        public int? Line { get; set; }
        public int? Column { get; set; }
        public IEntity Entity { get; set; }
        public AstNode AstNode { get; set; }
    }
}
