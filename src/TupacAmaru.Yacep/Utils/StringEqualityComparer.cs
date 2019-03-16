using System.Collections.Generic;

namespace TupacAmaru.Yacep.Utils
{
    internal sealed class StringEqualityComparer : IEqualityComparer<string>
    {
        private StringEqualityComparer() { }

        internal static readonly StringEqualityComparer Instance = new StringEqualityComparer();

        public bool Equals(string x, string y) => x.Equals(y);

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}
