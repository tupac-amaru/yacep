using System;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Expressions
{
    public sealed class LiteralExpression : EvaluableExpression
    {
        public LiteralValue LiteralValue { get; }
        public Type ValueType { get; }

        public LiteralExpression(LiteralValue literalValue, int startIndex, int endIndex) : base("Literal", startIndex, endIndex)
        {
            LiteralValue = literalValue;
            var value = literalValue.Value;
            ValueType = value == null ? typeof(object) : value.GetType();
        }
    }
}