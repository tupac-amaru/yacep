using TupacAmaru.Yacep.Evaluators;
using Xunit;

namespace TupacAmaru.Yacep.Test.Evaluators
{
    public class EvaluateInUnitTest
    {
        [Fact(DisplayName = "evaluate in expression")]
        public void InExpressionEvaluate()
        {
            Assert.Equal(true, InEvaluator.Evaluate(1, new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(true, InEvaluator.Evaluate(2.4f, new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(true, InEvaluator.Evaluate(true, new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(true, InEvaluator.Evaluate(null, new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(true, InEvaluator.Evaluate("a", new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(false, InEvaluator.Evaluate(3, new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(false, InEvaluator.Evaluate(4.4f, new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(false, InEvaluator.Evaluate(false, new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(false, InEvaluator.Evaluate(new object(), new object[] { 1, 2.4f, true, null, "a" }));
            Assert.Equal(false, InEvaluator.Evaluate("b", new object[] { 1, 2.4f, true, null, "a" }));

            Assert.Equal(false, InEvaluator.Evaluate(new object[] { 1, 2 }, new object[] { 1, 2.4f, true, null, "a", new object[] { 1, 2 } }));
        }
    }
}
