using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JSharp.Java.Ast
{
    class AutoDisposeIndentedTextWriter : System.CodeDom.Compiler.IndentedTextWriter
    {
        public AutoDisposeIndentedTextWriter(TextWriter writer)
            : base(writer)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (InnerWriter != null)
                InnerWriter.Dispose();
        }
    }

}
