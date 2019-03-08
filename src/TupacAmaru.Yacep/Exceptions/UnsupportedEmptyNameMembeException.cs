using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class UnsupportedEmptyNameMembeException : Exception
    {
        public UnsupportedEmptyNameMembeException(Type type, bool isIndexer)
            : base($"Not support empty name {(isIndexer ? "indexer" : "member")}({type.Name})") { }
    }
}
