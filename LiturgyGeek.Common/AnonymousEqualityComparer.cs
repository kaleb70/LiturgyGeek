using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common
{
    public sealed class AnonymousEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T?, T?, bool> equals;
        private readonly Func<T, int> getHashCode;

        public AnonymousEqualityComparer(Func<T?, T?, bool> equals, Func<T, int> getHashCode)
        {
            this.equals = equals;
            this.getHashCode = getHashCode;
        }

        public AnonymousEqualityComparer(Func<T?, T?, bool> equals)
            : this(equals, obj => 0)
        {
        }

        public bool Equals(T? x, T? y) => equals(x, y);

        public int GetHashCode([DisallowNull] T obj) => getHashCode(obj);
    }
}
