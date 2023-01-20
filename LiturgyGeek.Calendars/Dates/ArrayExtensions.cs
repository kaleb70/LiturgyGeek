using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    internal static class ArrayExtensions
    {
        public static int IndexOf<T>(this T[] array, T value, IEqualityComparer<T> equalityComparer)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (equalityComparer.Equals(array[i], value))
                    return i;
            }
            return -1;
        }
    }
}
