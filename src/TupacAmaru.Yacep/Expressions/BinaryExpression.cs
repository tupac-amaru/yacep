using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Expressions
{
    public sealed class BinaryExpression : EvaluableExpression
    {
        public BinaryOperator BinaryOperator { get; }
        public EvaluableExpression Left { get; }
        public EvaluableExpression Right { get; }

        public BinaryExpression(BinaryOperator binaryOperator, EvaluableExpression left, EvaluableExpression right, int startIndex, int endIndex) : base("Binary", startIndex, endIndex)
        {
            BinaryOperator = binaryOperator;
            Left = left;
            Right = right;
        }
    }
}