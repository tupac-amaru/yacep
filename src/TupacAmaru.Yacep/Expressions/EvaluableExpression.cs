namespace TupacAmaru.Yacep.Expressions
{
    public abstract class EvaluableExpression
    {
        public string TypeName { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }

        protected EvaluableExpression(string typeName, int startIndex, int endIndex)
        {
            TypeName = typeName;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }
}