using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public sealed class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(Type type, string methodName)
            : base($"Not found method({methodName}) in ({type.Name})") { }
    }
}
