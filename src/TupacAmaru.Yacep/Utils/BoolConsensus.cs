using System;

namespace TupacAmaru.Yacep.Utils
{
    public static class BoolConsensus
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
    }
}
