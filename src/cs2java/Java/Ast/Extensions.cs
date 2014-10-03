using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSharp.Java.Ast
{
    static class Extensions
    {
        public static IEnumerable<JNode> DescendantsAndSelf(this JNode node)
        {
            return GetDescendants(node, true);
        }
        public static IEnumerable<JNode> Descendants(this JNode node)
        {
            return GetDescendants(node, false);
        }
        public static IEnumerable<T> Descendants<T>(this JNode node) where T: JNode
        {
            return node.Descendants().OfType<T>();
        }
        static IEnumerable<JNode> GetDescendants(JNode node, bool self)
        {
            if (self)
                yield return node;
            var list = node.Children().ToList();
            for (var i = 0; i < list.Count; i++)
            {
                var child = list[i];
                yield return child;
                list.AddRange(child.Children());
            }
        }
    }
}
