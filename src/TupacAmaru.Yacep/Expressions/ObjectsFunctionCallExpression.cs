namespace TupacAmaru.Yacep.Expressions
{
    public sealed class ObjectsFunctionCallExpression : EvaluableExpression
    {
        public ObjectMemberExpression Callee { get; }
        public EvaluableExpression[] Arguments { get; }

        public ObjectsFunctionCallExpression(ObjectMemberExpression callee, EvaluableExpression[] arguments, int startIndex, int endIndex) : base("ObjectsFunctionCall", startIndex, endIndex)
        {
            Callee = callee;
            Arguments = arguments;
        }
    }
}