namespace TupacAmaru.Yacep
{
    public interface IEvaluator
    {
        object Evaluate(object state);
    }
    public interface IEvaluator<in TState>
    {
        object Evaluate(TState state);
    }
}