using System;

namespace TupacAmaru.Yacep.BuiltIn
{
    public static class Consensus
    {
        public static bool AsBool(this object value)
        {
            switch (value)
            {
                case bool boolValue:
                    return boolValue;
                case char charValue:
                    return 0 < charValue;
                case sbyte sbyteValue:
                    return 0 < sbyteValue;
                case byte byteValue:
                    return 0 < byteValue;
                case short shortValue:
                    return 0 < shortValue;
                case ushort ushortValue:
                    return 0 < ushortValue;
                case int intValue:
                    return 0 < intValue;
                case uint uintValue:
                    return 0 < uintValue;
                case long longValue:
                    return 0 < longValue;
                case ulong ulongValue:
                    return 0 < ulongValue;
                case float floatValue:
                    return 0 < floatValue;
                case double doubleValue:
                    return 0 < doubleValue;
                case decimal decimalValue:
                    return 0 < decimalValue;
                case string stringValue:
                    return !String.IsNullOrWhiteSpace(stringValue);
                default:
                    return value != null;
            }
        }

        internal static bool IsDigit(this object x) => x is char || x is sbyte || x is byte ||
                                                x is short || x is ushort || x is int || x is uint ||
                                                x is long || x is ulong || x is float || x is double || x is decimal;

        internal static int GetDigitTypeSize(this Type type)
        {
            if (type == typeof(sbyte) || type == typeof(byte)) return 1;
            if (type == typeof(char) || type == typeof(short) || type == typeof(ushort)) return 2;
            if (type == typeof(int) || type == typeof(uint) || type == typeof(float)) return 4;
            if (type == typeof(long) || type == typeof(ulong) || type == typeof(double)) return 8;
            return 16;
        }
    }
}
