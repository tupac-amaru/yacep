using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Expressions
{
    public sealed class NakedFunctionCallExpression : EvaluableExpression
    {
        public NakedFunction NakedFunction { get; }
        public EvaluableExpression[] Arguments { get; }

        public NakedFunctionCallExpression(NakedFunction nakedFunction, EvaluableExpression[] arguments, int startIndex, int endIndex) : base("NakedFunctionCall", startIndex, endIndex)
        {
            Arguments = arguments;
            NakedFunction = nakedFunction;
        }
    }
}