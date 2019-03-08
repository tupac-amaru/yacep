using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class UnsupportedFunctionException : Exception
    {
        public UnsupportedFunctionException()
            : base($"Not support function caller") { }
    }
}
