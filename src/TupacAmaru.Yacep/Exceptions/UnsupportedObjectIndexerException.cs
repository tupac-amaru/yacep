using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class UnsupportedObjectIndexerException : Exception
    {
        public UnsupportedObjectIndexerException(Type type, Type indexerType)
            : base($"Not support indexer({type.Name }),index argument type({indexerType.Name})") { }
    }
}
