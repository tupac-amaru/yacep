using System;
using System.Collections.Generic;
using System.Linq;

namespace TupacAmaru.Yacep.Exceptions
{
    public class UnsupportedFunctionCallException : Exception
    {
        public UnsupportedFunctionCallException(string functionName, IEnumerable<Type> types)
            : base($"Not support ({functionName}) for arguments ({string.Join(",", types.Select(x => x?.Name ?? "null").ToArray())})") { }
        public UnsupportedFunctionCallException(string functionName)
            : base($"Not support ({functionName}) for empty arguments") { }
    }
}
