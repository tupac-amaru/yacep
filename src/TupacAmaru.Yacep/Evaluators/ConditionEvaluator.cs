using TupacAmaru.Yacep.BuiltIn;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object ConditionExpressionEvaluator(object condition, object valueIfTrue, object valueIfFalse);
    public static class ConditionEvaluator
    {
        public static object Evaluate(object condition, object valueIfTrue, object valueIfFalse)
            => condition.AsBool() ? valueIfTrue : valueIfFalse;
    }
}
