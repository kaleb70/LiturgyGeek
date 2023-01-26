using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Collections
{
    public class ReadOnlyListEx<T>
    {
        public static IReadOnlyList<T> Empty { get; } = new List<T>();

        private ReadOnlyListEx()
        {
        }
    }
}
