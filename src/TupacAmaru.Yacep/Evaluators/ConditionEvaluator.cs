using TupacAmaru.Yacep.BuiltIn;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object ConditionExpressionEvaluator(object condition, object valueIfTrue, object valueIfFalse);
    public static class ConditionEvaluator
    {
        public static object Evaluate(object condition, object valueIfTrue, object valueIfFalse)
            => condition.AsBool() ? valueIfTrue : valueIfFalse;
    }
}
