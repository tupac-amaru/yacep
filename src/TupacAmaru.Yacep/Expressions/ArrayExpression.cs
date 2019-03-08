namespace TupacAmaru.Yacep.Expressions
{
    public sealed class ArrayExpression : EvaluableExpression
    {
        public EvaluableExpression[] Elements { get; }
        public ArrayExpression(EvaluableExpression[] elements, int startIndex, int endIndex) : base("Array", startIndex, endIndex) => Elements = elements;
    }
}