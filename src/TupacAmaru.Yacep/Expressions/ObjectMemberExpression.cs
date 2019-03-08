namespace TupacAmaru.Yacep.Expressions
{
    public sealed class ObjectMemberExpression : EvaluableExpression
    {
        public EvaluableExpression Object { get; }
        public EvaluableExpression Member { get; }
        public bool IsIndexer { get; }
        public ObjectMemberExpression(EvaluableExpression @object, EvaluableExpression member, int startIndex, int endIndex, bool isIndexer) : base(isIndexer ? "ObjectIndexer" : "ObjectMember", startIndex, endIndex)
        {
            Object = @object;
            Member = member;
            IsIndexer = isIndexer;
        }
    }
}