using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace JSharp.Compiler
{
    class QueuedConsole
    {
        public bool AutoFlush { get; set; }
        public ConcurrentQueue<string> Items = new ConcurrentQueue<string>();
        public void WriteLine(object p)
        {
            Items.Enqueue(p.ToString());
            DoAutoFlush();
        }

        private void DoAutoFlush()
        {
            if (!AutoFlush)
                return;
            Flush();
        }

        public void Flush()
        {
            while (Items.Count > 0)
            {
                string item;
                if (Items.TryDequeue(out item))
                {
                    Console.WriteLine(item);
                }
            }
        }
        public void WriteLine(string msg)
        {
            Items.Enqueue(msg);
            DoAutoFlush();
        }
        public void FormatLine(string msg, params object[] args)
        {
            Items.Enqueue(String.Format(msg, args));
            DoAutoFlush();
        }
    }

    //class Console
    //{
    //    public static bool AutoFlush { get; set; }
    //    public static ConcurrentQueue<string> Items = new ConcurrentQueue<string>();
    //    public static void WriteLine(object p)
    //    {
    //        Items.Enqueue(p.ToString());
    //        DoAutoFlush();
    //    }

    //    private static void DoAutoFlush()
    //    {
    //        if (!AutoFlush)
    //            return;
    //        Flush();
    //    }

    //    public static void Flush()
    //    {
    //        while (Items.Count > 0)
    //        {
    //            string item;
    //            if (Items.TryDequeue(out item))
    //            {
    //                System.Console.WriteLine(item);
    //            }
    //        }
    //    }
    //    public static void WriteLine(string msg)
    //    {
    //        Items.Enqueue(msg);
    //        DoAutoFlush();
    //    }
    //    public static void FormatLine(string msg, params object[] args)
    //    {
    //        Items.Enqueue(String.Format(msg, args));
    //        DoAutoFlush();
    //    }
    //}


}
