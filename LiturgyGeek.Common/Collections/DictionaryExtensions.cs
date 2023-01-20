using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Collections
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
                where TKey : notnull
                where TValue : ICloneable<TValue>
            => new(dictionary.Select(e => new KeyValuePair<TKey, TValue>(e.Key, e.Value.Clone())));

        public static Dictionary<TKey, TValue[]> Clone<TKey, TValue>(this Dictionary<TKey, TValue[]> dictionary)
                where TKey : notnull
                where TValue : ICloneable<TValue>
            => new(dictionary.Select(e => new KeyValuePair<TKey, TValue[]>(e.Key, e.Value.Select(i => i.Clone()).ToArray())));

        public static IEnumerable<KeyValuePair<TKey, TValue>> WithValues<TKey, TValue>(
                this Dictionary<TKey, TValue> dictionary,
                Func<KeyValuePair<TKey, TValue>, TValue> valueSelector)
        {
            return dictionary.Select(e => new KeyValuePair<TKey, TValue>(e.Key, valueSelector(e)));
        }
    }
}
