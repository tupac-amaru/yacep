using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Symbols;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.BuiltIn
{
    public static class BinaryOperators
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> addFunctions
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>>();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> minusFunctions
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>>();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> multiplyFunctions
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>>();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> divideFunctions
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>>();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> moduloFunctions
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>>();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, int>>> compareFunctions
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, int>>>();

        private static MethodInfo GetCompareTo(Type type) => type.GetMethod("CompareTo", new[] { type });
        private static MethodInfo GetConvertTo(Type fromType, Type toType) => typeof(Convert)
            .GetMethods(BindingFlags.Public | BindingFlags.Static).First(x =>
            {
                var name = x.Name;
                if (name != $"To{toType.Name}") return false;
                var first = x.GetParameters().First();
                return first.ParameterType == fromType;
            });
        private static Func<object, object, int> GetCompareTo(Type xType, Type yType)
        {
            return compareFunctions.GetOrAdd(xType, _ => new ConcurrentDictionary<Type, Func<object, object, int>>())
                .GetOrAdd(yType, _ =>
                {
                    var x = Expression.Parameter(typeof(object), "x");
                    var y = Expression.Parameter(typeof(object), "y");
                    Expression compare;
                    if (xType == yType)
                    {
                        compare = Expression.Call(xType.IsValueType ? Expression.Unbox(x, xType) : Expression.Convert(x, xType),
                            GetCompareTo(xType),
                            yType.IsValueType ? Expression.Unbox(y, yType) : Expression.Convert(y, yType));
                    }
                    else
                    {
                        var xTypeSize = xType.GetDigitTypeSize();
                        var yTypeSize = yType.GetDigitTypeSize();
                        var unionType = xTypeSize > yTypeSize ? xType : yType;
                        compare = Expression.Call(
                            xType == unionType ? (Expression)(xType.IsValueType ? Expression.Unbox(x, xType) : Expression.Convert(x, xType)) : Expression.Call(GetConvertTo(xType, unionType), xType.IsValueType ? Expression.Unbox(x, xType) : Expression.Convert(x, xType)),
                            GetCompareTo(unionType),
                            xType == unionType ? (Expression)Expression.Call(GetConvertTo(yType, unionType), yType.IsValueType ? Expression.Unbox(y, yType) : Expression.Convert(y, yType)) : (yType.IsValueType ? Expression.Unbox(y, yType) : Expression.Convert(y, yType)));
                    }
                    var function = Expression.Lambda<Func<object, object, int>>(compare, $"Compare<{xType.Name}>With<{yType.Name}>", new[] { x, y });
                    return function.Compile();
                });
        }
        private static Func<object, object, object> GetOrAddFunction(this ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> cache,
            Type xType, Type yType, Func<Expression, Expression, BinaryExpression> operate)
            => cache.GetOrAdd(xType, _ => new ConcurrentDictionary<Type, Func<object, object, object>>())
                .GetOrAdd(yType, _ =>
                {
                    var x = Expression.Parameter(typeof(object), "x");
                    var y = Expression.Parameter(typeof(object), "y");
                    Expression addAssign;
                    if (xType == yType)
                    {
                        addAssign = Expression.Convert(operate(Expression.Unbox(x, xType), Expression.Unbox(y, xType)), typeof(object));
                    }
                    else
                    {
                        var xTypeSize = xType.GetDigitTypeSize();
                        var yTypeSize = yType.GetDigitTypeSize();
                        var unionType = xTypeSize > yTypeSize ? xType : yType;
                        addAssign = Expression.Convert(operate(
                                xType == unionType ? (Expression)Expression.Unbox(x, xType) : Expression.Call(GetConvertTo(xType, unionType), Expression.Unbox(x, xType)),
                                yType == unionType ? (Expression)Expression.Unbox(y, yType) : Expression.Call(GetConvertTo(yType, unionType), Expression.Unbox(y, yType))),
                            typeof(object));
                    }
                    var function = Expression.Lambda<Func<object, object, object>>(addAssign, $"Add<{xType.Name}>And<{yType.Name}>", new[] { x, y });
                    return function.Compile();
                });
        private static int CompareTo(string @operator, object x, object y)
        {
            if (x == y) return 0;
            if (x == null || y == null)
                throw new UnsupportedOperationException(@operator, x, y);
            var xType = x.GetType();
            var yType = y.GetType();
            if (xType == yType)
                return GetCompareTo(xType, yType)(x, y);
            if (x is char charX)
                return GetCompareTo(typeof(int), yType)((int)charX, y);
            if (y is char charY)
                return GetCompareTo(xType, typeof(int))(x, (int)charY);
            if (x.IsDigit() && y.IsDigit())
                return GetCompareTo(xType, yType)(x, y);
            throw new UnsupportedOperationException(@operator, x, y);
        }

        private static bool ObjectEquals(object x, object y) => CompareTo("==", x, y) == 0;
        private static object OrHandler(object x, object y) => x.AsBool() || y.AsBool();
        private static object AndHandler(object x, object y) => x.AsBool() && y.AsBool();
        private static object EqualHandler(object x, object y) => ObjectEquals(x, y);
        private static object NotEqualHandler(object x, object y) => !ObjectEquals(x, y);
        private static object LessHandler(object x, object y) => CompareTo("<", x, y) < 0;
        private static object GreaterHandler(object x, object y) => CompareTo(">", x, y) > 0;
        private static object LessEqualHandler(object x, object y) => CompareTo("<=", x, y) <= 0;
        private static object GreaterEqualHandler(object x, object y) => CompareTo(">=", x, y) >= 0;
        private static object AddHandler(object x, object y)
        {
            if (x.IsDigit() && y.IsDigit())
                return addFunctions.GetOrAddFunction(x.GetType(), y.GetType(), Expression.Add)(x, y);
            if (x is string stringX && y is string stringY)
                return string.Concat(stringX, stringY);
            throw new UnsupportedOperationException("+", x, y);
        }
        private static object MinusHandler(object x, object y)
        {
            if (x.IsDigit() && y.IsDigit())
                return minusFunctions.GetOrAddFunction(x.GetType(), y.GetType(), Expression.Subtract)(x, y);
            throw new UnsupportedOperationException("-", x, y);
        }
        private static object MultiplyHandler(object x, object y)
        {
            if (x.IsDigit() && y.IsDigit())
                return multiplyFunctions.GetOrAddFunction(x.GetType(), y.GetType(), Expression.Multiply)(x, y);
            switch (x)
            {
                case int intX when y is string stringY:
                    return string.Join("", Enumerable.Repeat(stringY, intX));
                case string stringX when y is int intY:
                    return string.Join("", Enumerable.Repeat(stringX, intY));
                default:
                    throw new UnsupportedOperationException("*", x, y);
            }
        }
        private static object DivideHandler(object x, object y)
        {
            if (x.IsDigit() && y.IsDigit())
                return divideFunctions.GetOrAddFunction(x.GetType(), y.GetType(), Expression.Divide)(x, y);
            throw new UnsupportedOperationException("/", x, y);
        }
        private static object ModuloHandler(object x, object y)
        {
            if (x.IsDigit() && y.IsDigit())
                return moduloFunctions.GetOrAddFunction(x.GetType(), y.GetType(), Expression.Modulo)(x, y);
            throw new UnsupportedOperationException("%", x, y);
        }

        public static readonly BinaryOperator Or = new BinaryOperator("||", OrHandler, 1);
        public static readonly BinaryOperator And = new BinaryOperator("&&", AndHandler, 2);
        public static readonly BinaryOperator Equal = new BinaryOperator("==", EqualHandler, 3);
        public static readonly BinaryOperator NotEqual = new BinaryOperator("!=", NotEqualHandler, 3);
        public static readonly BinaryOperator Less = new BinaryOperator("<", LessHandler, 4);
        public static readonly BinaryOperator Greater = new BinaryOperator(">", GreaterHandler, 4);
        public static readonly BinaryOperator LessEqual = new BinaryOperator("<=", LessEqualHandler, 4);
        public static readonly BinaryOperator GreaterEqual = new BinaryOperator(">=", GreaterEqualHandler, 4);
        public static readonly BinaryOperator Add = new BinaryOperator("+", AddHandler, 5);
        public static readonly BinaryOperator Minus = new BinaryOperator("-", MinusHandler, 5);
        public static readonly BinaryOperator Multiply = new BinaryOperator("*", MultiplyHandler, 6);
        public static readonly BinaryOperator Divide = new BinaryOperator("/", DivideHandler, 6);
        public static readonly BinaryOperator Modulo = new BinaryOperator("%", ModuloHandler, 6);
    }
}
