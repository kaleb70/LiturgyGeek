using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common
{
    public static class HashCodeExtensions
    {
        public static void AddList<T>(this ref HashCode hashCode, List<T> list) => hashCode.AddList((IReadOnlyList<T>)list);

        public static void AddList<T>(this ref HashCode hashCode, IList<T> list)
        {
            foreach (var value in list)
                hashCode.Add(value);
        }

        public static void AddList<T>(this ref HashCode hashCode, IReadOnlyList<T> list)
        {
            foreach (var value in list)
                hashCode.Add(value);
        }

        public static void AddArray<T>(this ref HashCode hashCode, T[] array)
        {
            for (int i = 0; i < array.Length; i++)
                hashCode.Add(array[i]);
        }

        public static void AddSet<T>(this ref HashCode hashCode, IReadOnlySet<T> set)
        {
            foreach (var value in set.Order())
                hashCode.Add(value);
        }

        public static void AddDictionary<TKey, TValue>(this ref HashCode hashCode, IDictionary<TKey, TValue> dictionary)
        {
            foreach (var e in dictionary.OrderBy(d => d.Key))
            {
                hashCode.Add(e.Key);
                hashCode.Add(e.Value);
            }
        }
    }
}
