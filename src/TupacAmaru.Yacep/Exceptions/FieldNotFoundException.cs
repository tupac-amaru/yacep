using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public sealed class FieldNotFoundException : Exception
    {
        public FieldNotFoundException(Type type, string fieldName)
            : base($"Not found field({fieldName}) in ({type.Name})") { }
    }
}
