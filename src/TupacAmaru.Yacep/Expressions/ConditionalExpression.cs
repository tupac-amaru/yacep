namespace TupacAmaru.Yacep.Expressions
{
    public sealed class ConditionalExpression : EvaluableExpression
    {
        public EvaluableExpression Condition { get; }
        public EvaluableExpression ValueIfTrue { get; }
        public EvaluableExpression ValueIfFalse { get; }
        public ConditionalExpression(EvaluableExpression condition, EvaluableExpression valueIfTrue, EvaluableExpression valueIfFalse, int startIndex, int endIndex) : base("Conditional", startIndex, endIndex)
        {
            Condition = condition;
            ValueIfTrue = valueIfTrue;
            ValueIfFalse = valueIfFalse;
        }
    }
}