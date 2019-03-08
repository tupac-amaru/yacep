using System.Linq;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object InExpressionEvaluator(object value, object[] values);
    public static class InEvaluator
    {
        public static object Evaluate(object value, object[] values)
            => values.Contains(value);
    }
}
