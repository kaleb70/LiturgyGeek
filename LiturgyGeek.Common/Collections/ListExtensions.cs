using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Collections
{
    public static class ListExtensions
    {
        public static List<T> Clone<T>(this List<T> list) where T : ICloneable<T>
            => list.Select(e => e.Clone()).ToList();

        public static bool SubsetContains<T>(this List<T> list, T item, int startIndex, int count, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < count; i++)
            {
                if (comparer.Equals(item, list[startIndex + i]))
                    return true;
            }
            return false;
        }

        public static bool SubsetContains<T>(this List<T> list, T item, int startIndex, int count)
            => list.SubsetContains(item, startIndex, count, EqualityComparer<T>.Default);
    }
}
