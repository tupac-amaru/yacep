using System;
using System.Collections.Generic;

namespace TupacAmaru.Yacep.Collections
{
    internal sealed class StringEqualityComparer : IEqualityComparer<string>
    {
        private StringEqualityComparer() { }

        internal static readonly StringEqualityComparer Instance = new StringEqualityComparer();

        public bool Equals(string x, string y) => string.Equals(x, y, StringComparison.Ordinal);

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}
