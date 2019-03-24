using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep
{
    public interface ICompiler
    {
        IEvaluator Compile(EvaluableExpression expression);
        IEvaluator<TState> Compile<TState>(EvaluableExpression expression);
    }
}
