using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message) { }
    }
}
