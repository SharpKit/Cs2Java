using java.lang;
using java.util;

namespace system.linq
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this Collection<T> list, Iterable<T> items)
        {
            foreach (var item in items)
                list.add(item);
        }
    }
}
