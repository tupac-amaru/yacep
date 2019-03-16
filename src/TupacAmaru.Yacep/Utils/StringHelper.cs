using System.Linq;

namespace TupacAmaru.Yacep.Utils
{
    internal static class StringHelper
    {
        internal static bool ContainsSpace(this string str) => str.Any(CharHelper.IsSpace);
    }
}
