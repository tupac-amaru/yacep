using System;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Symbols
{
    public sealed class LiteralValue
    {
        public LiteralValue(string literal, object value)
        {
            if (literal.ContainsSpace())
                throw new CannotContainsSpacesException("Literal");
            if (value is Delegate)
                throw new UnsupportedLiteralValueException();
            Literal = literal;
            Value = value;
        }

        public string Literal { get; }
        public object Value { get; }
    }
}