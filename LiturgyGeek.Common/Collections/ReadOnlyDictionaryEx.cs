using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Collections
{
    public class ReadOnlyDictionaryEx<TKey, TValue> where TKey : notnull
    {
        public static IReadOnlyDictionary<TKey, TValue> Empty { get; } = new Dictionary<TKey, TValue>();

        public ReadOnlyDictionaryEx()
        {
        }
    }
}
