using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep
{
    public interface IParser
    {
        EvaluableExpression Parse(string expr, ReadOnlyParseOption option = null);
    }
}
