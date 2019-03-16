using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Symbols;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.BuiltIn
{
    public static class UnaryOperators
    {
        private static object NotHandler(object value) => !value.AsBool();
        private static object NegativeHandler(object value)
        {
            switch (value)
            {
                case char charValue:
                    return -charValue;
                case sbyte sbyteValue:
                    return -sbyteValue;
                case byte byteValue:
                    return -byteValue;
                case short shortValue:
                    return -shortValue;
                case ushort ushortValue:
                    return -ushortValue;
                case int intValue:
                    return -intValue;
                case uint uintValue:
                    return -uintValue;
                case long longValue:
                    return -longValue;
                case float floatValue:
                    return -floatValue;
                case double doubleValue:
                    return -doubleValue;
                case decimal decimalValue:
                    return -decimalValue;
                default:
                    throw new UnsupportedOperationException("-", value);
            }
        }
        private static object PositiveHandler(object value)
        {
            switch (value)
            {
                case char _:
                case sbyte _:
                case byte _:
                case short _:
                case ushort _:
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case float _:
                case double _:
                case decimal _:
                    return value;
                default:
                    throw new UnsupportedOperationException("+", value);
            }
        }

        public static readonly UnaryOperator Positive = new UnaryOperator("+", PositiveHandler);
        public static readonly UnaryOperator Negative = new UnaryOperator("-", NegativeHandler);
        public static readonly UnaryOperator Not = new UnaryOperator("!", NotHandler);
    }
}
