using TupacAmaru.Yacep.Expressions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.Expression
{
    public class ParseInExpressionUnitTest
    {
        [Fact(DisplayName = "parse in expression")]
        public void ParseInExpression()
        {
            var parser = new Core.Parser();

            var inExpression = parser.Parse("1 in (1,3)") as InExpression;
            Assert.NotNull(inExpression);
            Assert.Equal(1, (inExpression.Value as ConstantExpression)?.Value);
            Assert.Equal(2, inExpression.Values.Length);

            inExpression = parser.Parse("f.x in (a,b,c,true)") as InExpression;
            Assert.NotNull(inExpression);
            Assert.Equal("f", ((inExpression.Value as ObjectMemberExpression)?.Object as IdentifierExpression)?.Name);
            Assert.Equal("x", ((inExpression.Value as ObjectMemberExpression)?.Member as IdentifierExpression)?.Name);

            Assert.Equal(4, inExpression.Values.Length);
            Assert.Equal(true, (inExpression.Values[3] as LiteralExpression)?.LiteralValue.Value);

            inExpression = parser.Parse("(a+f.x) in (a,b,c,true)") as InExpression;
            Assert.NotNull(inExpression);
        }
    }
}
