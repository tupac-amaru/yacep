using System;

namespace TupacAmaru.Yacep.Extensions
{
    public static class EvaluatorExtension
    {
        private static object As(this object value, Type type)
        {
            if (value == null)
                return null;
            if (type.IsInstanceOfType(value))
            {
                return value;
            }
            if (type == typeof(string))
            {
                return value.ToString();
            }
            if (type == typeof(decimal))
            {
                return Convert.ToDecimal(value);
            }
            if (type.IsPrimitive)
            {
                return Convert.ChangeType(value, type);
            }
            if (type.IsEnum)
            {
                var enumValue = Enum.Parse(type, value.ToString());
                if (Enum.IsDefined(type, enumValue)) return enumValue;
                throw new FormatException($"can not cast {value} to {type.Name}");
            }
            throw new FormatException($"can not cast {value} to {type.Name}");
        }
        public static T EvaluateAs<T>(this IEvaluator evaluator, object state = null)
        {
            var value = evaluator.Evaluate(state);
            switch (value)
            {
                case null:
                    return default;
                case T tValue:
                    return tValue;
            }
            return (T)(value.As(typeof(T)));
        }

        public static object EvaluateAsType(this IEvaluator evaluator, object state, Type type) =>
            evaluator.Evaluate(state).As(type);

        public static object EvaluateAsType(this IEvaluator evaluator, Type type) =>
            evaluator.Evaluate(null).As(type);
    }
}
