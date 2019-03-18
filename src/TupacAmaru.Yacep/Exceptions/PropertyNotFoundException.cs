using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public sealed class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(Type type, string propertyName)
            : base($"Not found property({propertyName}) in ({type.Name}), or it can't read") { }
    }
}
