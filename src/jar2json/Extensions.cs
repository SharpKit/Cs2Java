using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharpTest
{
    static class Extensions
    {
        public static void ForEach<T>(this T[] list, Action<T> action)
        {
            foreach (var item in list)
                action(item);
        }

    }
}
