using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.BuiltIn
{
    public static class StatisticsFunctions
    {
        private static readonly ConcurrentDictionary<Type, Func<decimal, object, decimal>> addToDecimalFunctions
            = new ConcurrentDictionary<Type, Func<decimal, object, decimal>>();

        private static MethodInfo GetDecimalImplicit(Type type) => typeof(decimal)
            .GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x =>
            {
                var name = x.Name;
                if (name == "op_Implicit" || name == "op_Explicit")
                {
                    var first = x.GetParameters().First();
                    return first.ParameterType == type;
                }
                return false;
            });
        private static Func<decimal, object, decimal> GetAddToDecimalFunction(Type type) => addToDecimalFunctions.GetOrAdd(type, t =>
        {
            if (type == typeof(decimal)) return (x, y) => x + (decimal)y;

            var @implicit = GetDecimalImplicit(type);
            if (@implicit == null) return (x, y) => throw new UnsupportedOperationException("+", x, y);

            var a = Expression.Parameter(typeof(decimal), "x");
            var b = Expression.Parameter(typeof(object), "y");
            var addAssign = Expression.Add(a, type == typeof(decimal) ? (Expression)Expression.Unbox(b, type) :
                Expression.Call(@implicit, Expression.Unbox(b, type)));
            return Expression.Lambda<Func<decimal, object, decimal>>(addAssign, $"Add<{type.Name}>ToDecimal", new[] { a, b }).Compile();
        });
        private static object LenHandler(object[] arguments)
        {
            if (arguments.Length > 0)
            {
                switch (arguments[0])
                {
                    case ICollection collection:
                        return collection.Count;
                    case IEnumerable enumerable:
                        return enumerable.Cast<object>().Count();
                    default:
                        throw new UnsupportedFunctionCallException("len");
                }
            }
            throw new UnsupportedFunctionCallException("len");
        }
        private static object MaxHandler(object[] arguments)
        {
            var max = decimal.MinValue;
            var array = arguments.Length > 0 ? (arguments[0] as IEnumerable ?? new object[0]) : new object[0];
            var hasElement = false;
            foreach (var argument in array)
            {
                hasElement = true;
                switch (argument)
                {
                    case char charValue:
                        if (charValue > max)
                            max = charValue;
                        break;
                    case sbyte sbyteValue:
                        if (sbyteValue > max)
                            max = sbyteValue;
                        break;
                    case byte byteValue:
                        if (byteValue > max)
                            max = byteValue;
                        break;
                    case short shortValue:
                        if (shortValue > max)
                            max = shortValue;
                        break;
                    case ushort ushortValue:
                        if (ushortValue > max)
                            max = ushortValue;
                        break;
                    case int intValue:
                        if (intValue > max)
                            max = intValue;
                        break;
                    case uint uintValue:
                        if (uintValue > max)
                            max = uintValue;
                        break;
                    case long longValue:
                        if (longValue > max)
                            max = longValue;
                        break;
                    case ulong ulongValue:
                        if (ulongValue > max)
                            max = ulongValue;
                        break;
                    case float floatValue:
                        var convertedFloatValue = (decimal)floatValue;
                        if (convertedFloatValue > max)
                            max = convertedFloatValue;
                        break;
                    case double doubleValue:
                        var convertedDoubleValue = (decimal)doubleValue;
                        if (convertedDoubleValue > max)
                            max = convertedDoubleValue;
                        break;
                    case decimal decimalValue:
                        if (decimalValue > max)
                            max = decimalValue;
                        break;
                    default:
                        throw new UnsupportedFunctionCallException("max", arguments.Select(x => x?.GetType()));
                }
            }
            if (!hasElement) throw new UnsupportedFunctionCallException("max");
            return max;
        }
        private static object MinHandler(object[] arguments)
        {
            var min = decimal.MaxValue;
            var array = arguments.Length > 0 ? (arguments[0] as IEnumerable ?? new object[0]) : new object[0];
            var hasElement = false;
            foreach (var argument in array)
            {
                hasElement = true;
                switch (argument)
                {
                    case char charValue:
                        if (charValue < min)
                            min = charValue;
                        break;
                    case sbyte sbyteValue:
                        if (sbyteValue < min)
                            min = sbyteValue;
                        break;
                    case byte byteValue:
                        if (byteValue < min)
                            min = byteValue;
                        break;
                    case short shortValue:
                        if (shortValue < min)
                            min = shortValue;
                        break;
                    case ushort ushortValue:
                        if (ushortValue < min)
                            min = ushortValue;
                        break;
                    case int intValue:
                        if (intValue < min)
                            min = intValue;
                        break;
                    case uint uintValue:
                        if (uintValue < min)
                            min = uintValue;
                        break;
                    case long longValue:
                        if (longValue < min)
                            min = longValue;
                        break;
                    case ulong ulongValue:
                        if (ulongValue < min)
                            min = ulongValue;
                        break;
                    case float floatValue:
                        var convertedFloatValue = (decimal)floatValue;
                        if (convertedFloatValue < min)
                            min = convertedFloatValue;
                        break;
                    case double doubleValue:
                        var convertedDoubleValue = (decimal)doubleValue;
                        if (convertedDoubleValue < min)
                            min = convertedDoubleValue;
                        break;
                    case decimal decimalValue:
                        if (decimalValue < min)
                            min = decimalValue;
                        break;
                    default:
                        throw new UnsupportedFunctionCallException("min", arguments.Select(x => x?.GetType()));
                }
            }
            if (!hasElement) throw new UnsupportedFunctionCallException("min");
            return min;
        }
        private static object SumHandler(object[] arguments)
        {
            var array = arguments.Length > 0 ? (arguments[0] as IEnumerable ?? new object[0]) : new object[0];
            var hasElement = false;
            var sum = 0m;
            foreach (var argument in array)
            {
                hasElement = true;
                if (argument.IsDigit())
                {
                    sum = GetAddToDecimalFunction(argument.GetType())(sum, argument);
                }
                else
                {
                    throw new UnsupportedFunctionCallException("sum", arguments.Select(x => x?.GetType()));
                }
            }
            if (!hasElement) throw new UnsupportedFunctionCallException("sum");
            return sum;
        }
        private static object AvgHandler(object[] arguments)
        {
            var sum = 0m;
            var array = arguments.Length > 0 ? (arguments[0] as IEnumerable ?? new object[0]) : new object[0];
            var length = 0;
            foreach (var argument in array)
            {
                length++;
                if (argument.IsDigit())
                {
                    sum = GetAddToDecimalFunction(argument.GetType())(sum, argument);
                }
                else
                {
                    throw new UnsupportedFunctionCallException("avg", arguments.Select(x => x?.GetType()));
                }
            }
            if (length == 0) throw new UnsupportedFunctionCallException("avg");
            return sum / length;
        }
        public static readonly NakedFunction Len = new NakedFunction("len", LenHandler, true);
        public static readonly NakedFunction Max = new NakedFunction("max", MaxHandler, true);
        public static readonly NakedFunction Min = new NakedFunction("min", MinHandler, true);
        public static readonly NakedFunction Sum = new NakedFunction("sum", SumHandler, true);
        public static readonly NakedFunction Avg = new NakedFunction("avg", AvgHandler, true);
    }
}
