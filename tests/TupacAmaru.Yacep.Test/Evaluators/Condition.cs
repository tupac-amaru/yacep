using TupacAmaru.Yacep.Evaluators;
using Xunit;

namespace TupacAmaru.Yacep.Test.Evaluators
{
    public class EvaluateConditionUnitTest
    {
        [Fact(DisplayName = "evaluate condition expression")]
        public void ConditionExpressionEvaluate()
        {
            Assert.Equal(1, ConditionEvaluator.Evaluate(true, 1, 2));
            Assert.Equal(1, ConditionEvaluator.Evaluate(1, 1, 2));
            Assert.Equal(1, ConditionEvaluator.Evaluate(2.3f, 1, 2));
            Assert.Equal(1, ConditionEvaluator.Evaluate("aa", 1, 2));
            Assert.Equal(1, ConditionEvaluator.Evaluate(new object(), 1, 2));
            Assert.Equal(2, ConditionEvaluator.Evaluate(false, 1, 2));
            Assert.Equal(2, ConditionEvaluator.Evaluate(0, 1, 2));
            Assert.Equal(2, ConditionEvaluator.Evaluate(-9, 1, 2));
            Assert.Equal(2, ConditionEvaluator.Evaluate(-0f, 1, 2));
            Assert.Equal(2, ConditionEvaluator.Evaluate(-9.0f, 1, 2));
            Assert.Equal(2, ConditionEvaluator.Evaluate(null, 1, 2));
            Assert.Equal(2, ConditionEvaluator.Evaluate("", 1, 2));
        }
    }
}
