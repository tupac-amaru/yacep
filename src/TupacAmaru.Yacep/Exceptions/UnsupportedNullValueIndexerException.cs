using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class UnsupportedNullValueIndexerException : Exception
    {
        public UnsupportedNullValueIndexerException(Type type)
            : base($"Not support indexer({type.Name }) with null value") { }
    }
    public class CannotContainsSpacesException : Exception
    {
        public CannotContainsSpacesException(string name)
            : base($"{name} can not contains spaces") { }
    }
}
