using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class Extensions
    {
        public static T GetOrAdd<K, T>(this IDictionary<K, T> dic, K key, Func<K, T> func)
        {
            T value;
            if (dic.TryGetValue(key, out value))
                return value;
            value = func(key);
            dic.Add(key, value);
            return value;
        }

        public static void InsertRange<T>(this IList<T> set, int index, IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                set.Insert(index, item);
                index++;
            }
        }
        public static void SetItems<T>(this IList<T> list, IEnumerable<T> items)
        {
            list.Clear();
            list.AddRange(items);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.IsEmpty();
        }
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list != null && !list.IsEmpty();
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNotNullOrEmpty<T>(this IList<T> list)
        {
            return list != null && list.Count > 0;
        }
        public static T GetValue<K, T>(this Dictionary<K, T> dic, K key, Func<K, T> notFoundHandler)
        {
            T value;
            if (!dic.TryGetValue(key, out value))
            {
                return notFoundHandler(key);
            }
            return value;
        }

        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
                return false;
            dic.Add(key, value);
            return true;
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            if (dic.ContainsKey(key))
            {
                dic.Remove(key);
                return true;
            }
            return false;
        }

        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            dic.TryGetValue(key, out value);
            return value;
        }

        public static T GetCreate<K, T>(this Dictionary<K, T> dic, K key) where T : class, new()
        {
            var value = dic.TryGetValue(key);
            if (value == null)
            {
                value = new T();
                dic[key] = value;
            }
            return value;
        }


    }
}
