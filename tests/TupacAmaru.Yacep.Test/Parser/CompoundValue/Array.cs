using System.Linq;
using TupacAmaru.Yacep.Expressions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.CompoundValue
{
    public class ParseArrayExpressionUnitTest
    {
        [Fact(DisplayName = "parse array expression")]
        public void ParseArray()
        {
            var parser = new Core.Parser();

            var arrayExpression = parser.Parse("[1, 2.0222 ,true,'das']") as ArrayExpression;
            Assert.NotNull(arrayExpression);
            Assert.Equal(4, arrayExpression.Elements.Length);

            Assert.Equal(0, arrayExpression.StartIndex);
            Assert.Equal(23, arrayExpression.EndIndex);

            Assert.Equal(1, ((ConstantExpression)arrayExpression.Elements[0]).Value);
            Assert.Equal(2.0222m, ((ConstantExpression)arrayExpression.Elements[1]).Value);
            Assert.Equal(true, ((LiteralExpression)arrayExpression.Elements[2]).LiteralValue.Value);
            Assert.Equal("das", ((ConstantExpression)arrayExpression.Elements[3]).Value);

            arrayExpression = parser.Parse("[]") as ArrayExpression;
            Assert.NotNull(arrayExpression);
            Assert.Empty(arrayExpression.Elements);

            arrayExpression = parser.Parse($"[{string.Join(",", Enumerable.Range(1, 1000))}]") as ArrayExpression;
            Assert.NotNull(arrayExpression);
            Assert.Equal(1000, arrayExpression.Elements.Length);
        }
    }
}
