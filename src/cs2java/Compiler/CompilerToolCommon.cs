using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSharp.Java.Ast;
using System.IO;

namespace JSharp.Compiler
{
    static class ListExtensions
    {
        public static void RemoveDoubles<T>(this List<T> list, T item) where T : class
        {
            var i = 0;
            var count = 0;
            while (i < list.Count)
            {
                var item2 = list[i];
                if (item2 == item)
                {
                    count++;
                    if (count > 1)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }
                i++;
            }
        }
        public static void RemoveDoublesByKey<K, T>(this IList<T> list, Func<T, K> keySelector)
            where T : class
            where K : class
        {
            var set = new HashSet<K>();
            var i = 0;
            while (i < list.Count)
            {
                var item = list[i];
                var key = keySelector(item);
                if (key != null)
                {
                    if (set.Contains(key))
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    else
                    {
                        set.Add(key);
                    }
                }
                i++;
            }
        }

    }


    class CodeInjection
    {
        public CodeInjection()
        {
            Dependencies = new List<CodeInjection>();
        }
        public List<CodeInjection> SelfAndDependencies()
        {
            var list = new List<CodeInjection> { this };
            if (Dependencies.IsNotNullOrEmpty())
            {
                foreach (var dep in Dependencies)
                {
                    list.AddRange(dep.SelfAndDependencies());
                }
            }
            return list;
        }
        public string JsCode { get; set; }
        public string FunctionName { get; set; }
        public JStatement JsStatement { get; set; }
        public List<CodeInjection> Dependencies { get; set; }
    }

    class CompilerEvent
    {
        public CompilerEvent(Action before, Action action, Action after)
        {
            Before = before;
            Action = action;
            After = after;
        }
        public CompilerEvent(Action action)
        {
            Action = action;
        }
        public Action Before { get; set; }
        public Action Action { get; set; }
        public Action After { get; set; }
    }
}
