using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class UnsupportedLiteralValueException : Exception
    {
        public UnsupportedLiteralValueException()
            : base($"Not support Delegate type literal value") { }
    }
}
