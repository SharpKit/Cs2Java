using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SystemTools.Text
{

    public class CodeWriter : LineWriterProxy
    {
        public void WriteComma() { Write(","); }
        public void WriteDot() { Write("."); }
        public void WriteSemicolon() { Write(";"); }
        public void WriteEquals() { Write("="); }
        public int IndentSize
        {
            get
            {
                return 4;
            }
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
            if(Indent>0)
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
