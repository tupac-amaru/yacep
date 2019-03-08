using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep
{
    public interface IFormatter
    {
        string Format(EvaluableExpression expression);
    }
}
