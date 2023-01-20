using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Collections
{
    public static class HashSetExtensions
    {
        public static int AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
        {
            int result = 0;
            foreach (var item in collection)
            {
                if (hashSet.Add(item))
                    ++result;
            }
            return result;
        }
    }
}
