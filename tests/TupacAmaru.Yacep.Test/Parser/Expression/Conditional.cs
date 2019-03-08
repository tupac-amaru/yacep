using System.Linq;
using TupacAmaru.Yacep.Expressions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.Expression
{
    public class ParseConditionalExpressionUnitTest
    {
        [Fact(DisplayName = "parse conditional expression")]
        public void ParseConditionalExpression()
        {
            var parser = new Core.Parser();

            var conditionalExpression = parser.Parse("x ?a:b") as ConditionalExpression;
            Assert.NotNull(conditionalExpression);

            Assert.Equal("x", (conditionalExpression.Condition as IdentifierExpression)?.Name);
            Assert.Equal("a", (conditionalExpression.ValueIfTrue as IdentifierExpression)?.Name);
            Assert.Equal("b", (conditionalExpression.ValueIfFalse as IdentifierExpression)?.Name);

            conditionalExpression = parser.Parse("12?true:max($a)") as ConditionalExpression;
            Assert.NotNull(conditionalExpression);
            Assert.Equal(12, ((conditionalExpression.Condition as ConstantExpression)?.Value));
            Assert.Equal(true, ((conditionalExpression.ValueIfTrue as LiteralExpression)?.LiteralValue.Value));
            var callExpression = conditionalExpression.ValueIfFalse as NakedFunctionCallExpression;
            Assert.Equal("max", callExpression?.NakedFunction.Name);
            Assert.Equal("$a", (callExpression?.Arguments.First() as IdentifierExpression)?.Name);
        }
    }
}
