using java.lang;
using java.util;
using JSharp;

namespace system.linq
{
    public static class Enumerable
    {
        public static Iterable<T> ToIterable<T>(this Enumeration<T> list)
        {
            return new EnumerationIterator<T>(list);
        }
        public static Iterable<T> Where<T>(this Iterable<T> list, Func<T, bool> func)
        {
            return new WhereIterator<T>(list, func);
        }
        public static Iterable<R> Select<T, R>(this Iterable<T> list, Func<T, R> func)
        {
            return new SelectIterator<T, R>(list, func);
        }
        public static void ForEach<T>(this Iterable<T> list, Action<T> action)
        {
            foreach (var item in list)
                action(item);
        }
        public static List<T> ToList<T>(this Iterable<T> list)
        {
            if (list is List<Q>)
            {
                var list3 = (List<T>)list;
                return new ArrayList<T>(list3.As<Collection<T>>());
            }
            var list2 = new ArrayList<T>();
            foreach (var item in list)
                list2.add(item);
            return list2;
        }
        public static List<T> ToList<T>(this T[] list)
        {
            var list2 = new ArrayList<T>(list.Length);
            foreach (var item in list)
                list2.add(item);
            return list2;
        }
        static int Compare<T>(T x, T y) where T : Comparable<T>
        {
            if (x.As<object>() == null)
            {
                if (y.As<object>() != null)
                    return 1;
                return 0;
            }
            return x.compareTo(y);
        }
        
        public static List<T> OrderBy<T, R>([Final] this List<T> list, [Final] Func<T, R> selector) where R : Comparable<R>
        {
            var list2 = list.ToList();
            var xx = new MyComperator<T>((x, y) => Compare(selector(x), selector(y)));
            Collections.sort(list2, xx.As<Comparator<object>>());
            return list2;
        }
    }


    class MyComperator<T> : Object, Comparator<T>
    {
        public MyComperator(Func<T, T, int> compare)
        {
            Compare = compare;
        }

        public Func<T, T, int> Compare { get; set; }
        #region Comparator<T> Members


        public int compare(T prm1, T prm2)
        {
            return Compare(prm1, prm2);
        }

        #endregion
    }

}

