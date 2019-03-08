using TupacAmaru.Yacep.Expressions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.CompoundValue
{
    public class ParseObjectExpressionUnitTest
    {
        [Fact(DisplayName = "parse object expression")]
        public void ParseObjectExpression()
        {
            var parser = new Core.Parser();

            var callExpression = parser.Parse("x.func(a,b)") as ObjectsFunctionCallExpression;
            Assert.NotNull(callExpression);

            var objectMemberExpression = callExpression.Callee;
            Assert.NotNull(objectMemberExpression);
            Assert.False(objectMemberExpression.IsIndexer);
            Assert.Equal("func", (objectMemberExpression.Member as IdentifierExpression)?.Name);
            Assert.Equal("x", (objectMemberExpression.Object as IdentifierExpression)?.Name);

            Assert.Equal(2, callExpression.Arguments.Length);
            Assert.Equal("a", (callExpression.Arguments[0] as IdentifierExpression)?.Name);
            Assert.Equal("b", (callExpression.Arguments[1] as IdentifierExpression)?.Name);

            callExpression = parser.Parse("d['func'](a,b)") as ObjectsFunctionCallExpression;
            Assert.NotNull(callExpression);

            var objectIndexerExpression = callExpression.Callee;
            Assert.NotNull(objectIndexerExpression);
            Assert.True(objectIndexerExpression.IsIndexer);
            Assert.Equal("func", (objectIndexerExpression.Member as ConstantExpression)?.Value);
            Assert.Equal("d", (objectIndexerExpression.Object as IdentifierExpression)?.Name);
        }
    }
}
