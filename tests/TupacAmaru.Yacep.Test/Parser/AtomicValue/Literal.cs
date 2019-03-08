using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.AtomicValue
{
    public class ParseLiteralsUnitTest
    {
        [Fact(DisplayName = "parse literal expression")]
        public void ParseLiterals()
        {
            var parser = new Core.Parser();

            var literalExpression = parser.Parse("true") as LiteralExpression;
            Assert.NotNull(literalExpression);
            Assert.True((bool)literalExpression.LiteralValue.Value);

            literalExpression = parser.Parse("false") as LiteralExpression;
            Assert.NotNull(literalExpression);
            Assert.False((bool)literalExpression.LiteralValue.Value);

            literalExpression = parser.Parse("null") as LiteralExpression;
            Assert.NotNull(literalExpression);
            Assert.Null(literalExpression.LiteralValue.Value);

            var option = ParseOption.CreateOption()
                .AddLiteralValue("longValue1", 2147483649L)
                .AddLiteralValue("longValue2", 2147483641L)
                .AsReadOnly();

            literalExpression = parser.Parse("longValue1", option) as LiteralExpression;
            Assert.NotNull(literalExpression);
            Assert.Equal(2147483649L, literalExpression.LiteralValue.Value);

            literalExpression = parser.Parse("longValue2", option) as LiteralExpression;
            Assert.NotNull(literalExpression);
            Assert.Equal(2147483641L, literalExpression.LiteralValue.Value);
        }
    }
}
