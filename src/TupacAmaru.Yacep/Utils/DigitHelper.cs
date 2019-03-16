using System;

namespace TupacAmaru.Yacep.Utils
{
    internal static class DigitHelper
    {
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
