using System.Collections.Generic;
using TupacAmaru.Yacep.Evaluators;
using TupacAmaru.Yacep.Exceptions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Evaluators
{
    public class EvaluateObjectMemberUnitTest
    {
        [Fact(DisplayName = "evaluate object member expression")]
        public void ObjectMemberExpressionEvaluator()
        {
            Assert.Null(ObjectMemberEvaluator.Evaluate(null, 3, true));
            Assert.Equal(2, ObjectMemberEvaluator.Evaluate(new object[] { 1, 2, 3, 4 }, 1, true));
            Assert.Equal(4, ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, 3, true));
            Assert.Equal("abc", ObjectMemberEvaluator.Evaluate(new Dictionary<int, object>
            {
                [1] = "abc",
                [2] = "b"
            }, 1, true));
            Assert.Equal("aaa", ObjectMemberEvaluator.Evaluate(new Fixture() { x = 12, y = "aaa" }, "y", true));
            Assert.Throws<UnsupportedNullValueIndexerException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, null, true));
            Assert.Throws<UnsupportedEmptyNameMembeException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, "", true));
            Assert.Throws<UnsupportedObjectIndexerException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, true, true));

            Assert.Equal("aaa", ObjectMemberEvaluator.Evaluate(new Fixture() { x = 12, y = "aaa" }, "y", false));
            Assert.Throws<UnsupportedEmptyNameMembeException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, "", false));
        }
    }
}
