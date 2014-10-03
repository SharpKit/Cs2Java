using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JSharp.Java.Ast
{
    class LineWriterProxy
    {
        public int Indent
        {
            get
            {
                return InnerWriter.Indent;
            }
            set
            {
                InnerWriter.Indent = value;
            }
        }
        [DebuggerStepThrough]
        public void Indented(Action action)
        {
            Indent++;
            action();
            Indent--;
        }

        public void Write(string s)
        {
            InnerWriter.Write(s);
        }
        public void WriteLine()
        {
            InnerWriter.WriteLine();
        }

        public void Close()
        {
            InnerWriter.Close();
        }
        public int CurrentLine { get { return InnerWriter.CurrentLine; } }
        public int CurrentColumn { get { return InnerWriter.CurrentColumn; } }

        protected LineWriter InnerWriter { get; set; }
    }
}
