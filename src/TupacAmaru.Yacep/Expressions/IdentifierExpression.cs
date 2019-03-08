namespace TupacAmaru.Yacep.Expressions
{
    public sealed class IdentifierExpression : EvaluableExpression
    {
        public string Name { get; }

        public IdentifierExpression(string name, int startIndex, int endIndex) : base("Identifier", startIndex, endIndex) => Name = name;
    }
}