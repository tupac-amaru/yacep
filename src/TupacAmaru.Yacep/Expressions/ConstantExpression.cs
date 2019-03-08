using System;

namespace TupacAmaru.Yacep.Expressions
{
    public sealed class ConstantExpression : EvaluableExpression
    {
        public string Raw { get; }
        public object Value { get; }
        public Type ValueType { get; }

        public ConstantExpression(string raw, object value, int startIndex, int endIndex) : base("Constant", startIndex, endIndex)
        {
            Raw = raw;
            Value = value;
            ValueType = Value.GetType();
        }
    }
}