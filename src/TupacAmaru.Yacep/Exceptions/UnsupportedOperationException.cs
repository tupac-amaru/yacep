using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class UnsupportedOperationException : Exception
    {
        public UnsupportedOperationException(string @operator, object a, object b)
         : base($"Not support ({@operator}) for two type({{{a?.GetType().Name ?? "null"},{b?.GetType().Name ?? "null"}}})") { }

        public UnsupportedOperationException(string @operator, object a)
        : base($"Not support ({@operator}) for type({{{a?.GetType().Name ?? "null"}}})") { }
    }
}
