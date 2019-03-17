using System.Collections.Generic;
using System.Reflection;

namespace TupacAmaru.Yacep.Collections
{
    internal sealed class MethodInfoEqualityComparer : IEqualityComparer<MethodInfo>
    {
        private MethodInfoEqualityComparer() { }

        internal static readonly MethodInfoEqualityComparer Instance = new MethodInfoEqualityComparer();

        public bool Equals(MethodInfo x, MethodInfo y) => x == y;

        public int GetHashCode(MethodInfo obj) => obj.GetHashCode();
    }
}
