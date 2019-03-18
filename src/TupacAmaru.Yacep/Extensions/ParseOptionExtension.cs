using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Extensions
{
    public static class ParseOptionExtension
    {
        public static ParseOption NotAllowedArrayExpression(this ParseOption option)
        {
            option.NotAllowedArrayExpression = true;
            return option;
        }
        public static ParseOption NotAllowedConditionExpression(this ParseOption option)
        {
            option.NotAllowedConditionExpression = true;
            return option;
        }
        public static ParseOption NotAllowedMemberExpression(this ParseOption option)
        {
            option.NotAllowedMemberExpression = true;
            return option;
        }
        public static ParseOption NotAllowedIndexerExpression(this ParseOption option)
        {
            option.NotAllowedIndexerExpression = true;
            return option;
        }
        public static ParseOption NotAllowedInExpression(this ParseOption option)
        {
            option.NotAllowedInExpression = true;
            return option;
        }
        public static ParseOption NotAllowedConvertUnsignedInteger(this ParseOption option)
        {
            option.NotAllowedConvertUnsignedInteger = true;
            return option;
        }

        public static ParseOption AddUnaryOperator(this ParseOption option, UnaryOperator @operator)
        {
            option.UnaryOperators.Add(@operator);
            return option;
        }
        public static ParseOption AddUnaryOperator(this ParseOption option, string @operator, UnaryOperatorHandler handler)
            => option.AddUnaryOperator(new UnaryOperator(@operator, handler));

        public static ParseOption AddBinaryOperator(this ParseOption option, BinaryOperator @operator)
        {
            option.BinaryOperators.Add(@operator);
            return option;
        }
        public static ParseOption AddBinaryOperator(this ParseOption option, string @operator, BinaryOperatorHandler handler, uint precedence)
            => option.AddBinaryOperator(new BinaryOperator(@operator, handler, precedence));

        public static ParseOption AddLiteralValue(this ParseOption option, LiteralValue literalValue)
        {
            option.LiteralValues.Add(literalValue);
            return option;
        }
        public static ParseOption AddLiteralValue(this ParseOption option, string literal, object value)
            => option.AddLiteralValue(new LiteralValue(literal, value));

        public static ParseOption AddNakedFunction(this ParseOption option, NakedFunction function)
        {
            option.NakedFunctions.Add(function);
            return option;
        }
        public static ParseOption AddNakedFunction(this ParseOption option, string name, NakedFunctionHandler handler, bool cacheable = false)
            => option.AddNakedFunction(new NakedFunction(name, handler, cacheable));
    }
}
