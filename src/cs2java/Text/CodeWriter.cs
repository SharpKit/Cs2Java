using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace JSharp.Java.Ast
{

    class CodeWriter : LineWriterProxy
    {
        public void WriteComma() { Write(","); }
        public void WriteDot() { Write("."); }
        public void WriteSemicolon() { Write(";"); }
        public void WriteEquals() { Write("="); }
        public int IndentSize
        {
            get
            {
                return AutoDisposeIndentedTextWriter.DefaultTabString.Length;
            }
        }

        public void WriteLine(string token)
        {
            InnerWriter.Write(token);
            InnerWriter.WriteLine();
        }

        public void BeginBlock(bool singleLine = false)
        {
            if (singleLine)
                Write("{");
            else
            {
                Write("{");
                WriteLine();
            }
            Indent++;
        }
        public void EndBlock(bool singleLine = false)
        {
            Indent--;
            if (singleLine)
                Write("}");
            else
            {
                Write("}");
                WriteLine();
            }
        }
        [DebuggerStepThrough]
        public void Block(Action action)
        {
            BeginBlock();
            action();
            EndBlock();
        }



    }



}
