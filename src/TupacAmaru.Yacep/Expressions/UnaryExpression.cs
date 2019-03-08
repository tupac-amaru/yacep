using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Expressions
{
    public sealed class UnaryExpression : EvaluableExpression
    {
        public UnaryOperator UnaryOperator { get; }
        public EvaluableExpression Argument { get; }

        public UnaryExpression(UnaryOperator unaryOperator, EvaluableExpression argument, int startIndex, int endIndex) : base("Unary", startIndex, endIndex)
        {
            Argument = argument;
            UnaryOperator = unaryOperator;
        }
    }
}