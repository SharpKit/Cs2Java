using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SystemTools.Text
{
    public class LineWriterProxy
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
        public void Write(string s, object arg)
        {
            InnerWriter.Write(s, arg);
        }
        public void WriteLine()
        {
            InnerWriter.WriteLine();
        }
        public void WriteLine(string s)
        {
            InnerWriter.WriteLine(s);
        }
        public void WriteLine(string s, object arg)
        {
            InnerWriter.WriteLine(s, arg);
        }
        public void WriteLine(string s, params object[] args)
        {
            InnerWriter.WriteLine(s, args);
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
