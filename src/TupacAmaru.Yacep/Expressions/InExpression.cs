namespace TupacAmaru.Yacep.Expressions
{
    public sealed class InExpression : EvaluableExpression
    {
        public EvaluableExpression Value { get; }
        public EvaluableExpression[] Values { get; }

        public InExpression(EvaluableExpression value, EvaluableExpression[] values, int startIndex, int endIndex) : base("In", startIndex, endIndex)
        {
            Value = value;
            Values = values;
        }
    }
}