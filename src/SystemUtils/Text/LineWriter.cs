using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SystemTools.Text
{
    public class LineWriter : IDisposable
    {
        public LineWriter(TextWriter writer)
        {
            InnerWriter = writer;
            CurrentLine = 1;
            CurrentLineBuffer = new StringBuilder();
        }
        string TabString = "    ";

        public TextWriter InnerWriter { get; private set; }

        int IndentSize
        {
            get
            {
                return TabString.Length;
            }
        }
        int _Indent;
        public int Indent
        {
            get
            {
                return _Indent;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Indent");
                _Indent = value;
                if (IsInNewLine())
                    CurrentLineBuffer.Clear();
                if (CurrentLineBuffer.Length == 0 && Indent > 0)
                {
                    AppendTabs();
                }
            }
        }
        public bool IsInNewLine()
        {
            return String.IsNullOrWhiteSpace(CurrentLineBuffer.ToString());
        }

        private void AppendTabs()
        {
            for (var i = 0; i < Indent; i++)
                CurrentLineBuffer.Append(TabString);
        }
        public void Close()
        {
            if (!String.IsNullOrWhiteSpace(CurrentLineBuffer.ToString()))
                FlushCurrentLineBuffer();
            InnerWriter.Close();
        }
        public int CurrentLine { get; private set; }
        public int CurrentColumn
        {
            get { return CurrentLineBuffer.Length + 1; }
        }
        public int CurrentColumnWithoutNewLine
        {
            get
            {
                if (IsInNewLine())
                    return 1;
                return CurrentLineBuffer.Length + 1;
            }
        }
        public void WriteLine(string s)
        {
            CurrentLineBuffer.Append(s);
            FlushCurrentLineBuffer();
        }

        string NewLine { get { return InnerWriter.NewLine; } }

        public void WriteLine(string format, object arg0)
        {
            CurrentLineBuffer.AppendFormat(format, arg0);
            FlushCurrentLineBuffer();
        }

        public void WriteLine(string format, params object[] args)
        {
            CurrentLineBuffer.AppendFormat(format, args);
            FlushCurrentLineBuffer();
        }


        public void WriteLine()
        {
            FlushCurrentLineBuffer();
        }

        public void Write(string format, object arg0)
        {
            CurrentLineBuffer.AppendFormat(format, arg0);
        }

        public void Write(string s)
        {
            if (s == null)
                return;
            CurrentLineBuffer.Append(s);
        }

        public void Write(object obj)
        {
            CurrentLineBuffer.Append(obj);
        }


        public void Flush()
        {
            FlushCurrentLineBuffer(false);
        }
        StringBuilder CurrentLineBuffer;

        void FlushCurrentLineBuffer(bool newLine = true)
        {
            var s = CurrentLineBuffer.ToString();
            if (s.Contains(NewLine) || s.Contains("\n"))
            {
                var lines = s.Split(new string[] { NewLine, "\n" }, StringSplitOptions.None);
                var i = 0;
                foreach (var line in lines)
                {
                    if (!newLine && lines.Length - 1 == i)
                    {
                        InnerWriter.Write(line);
                    }
                    else
                    {
                        InnerWriter.WriteLine(line);
                        CurrentLine++;
                    }
                    i++;
                }
            }
            else
            {
                if (!newLine)
                {
                    InnerWriter.Write(s);
                }
                else
                {
                    InnerWriter.WriteLine(s);
                    CurrentLine++;
                }
            }
            CurrentLineBuffer.Clear();
            if (newLine)
                AppendTabs();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (CurrentLineBuffer != null && !String.IsNullOrWhiteSpace(CurrentLineBuffer.ToString()))
                FlushCurrentLineBuffer();
            if (InnerWriter != null)
                InnerWriter.Dispose();
        }

        #endregion
    }
}
