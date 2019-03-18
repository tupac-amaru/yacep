using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object ObjectMemberExpressionEvaluator(object @object, object member, bool isIndexer);
    public static class ObjectMemberEvaluator
    {
        public static object Evaluate(object @object, object member, bool isIndexer)
        {
            if (@object == null) return null;
            if (member == null)
            {
                throw new UnsupportedNullValueIndexerException(@object.GetType());
            }
            if (isIndexer)
            {
                if (member is string memberName)
                {
                    if (string.IsNullOrWhiteSpace(memberName))
                    {
                        throw new UnsupportedEmptyNameMembeException(@object.GetType(), true);
                    }
                    return @object.GetType().GetValue(memberName)(@object);
                }
                var type = @object.GetType();
                var indexerType = member.GetType();
                return type.CreateIndexer(indexerType)(@object, member);
            }
            else
            {
                var memberName = member as string;
                if (string.IsNullOrWhiteSpace(memberName))
                    throw new UnsupportedEmptyNameMembeException(@object.GetType(), false);
                return @object.GetType().GetValue(memberName)(@object);
            }
        }
    }
}
