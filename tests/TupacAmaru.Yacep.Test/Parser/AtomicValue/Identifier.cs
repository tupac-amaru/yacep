using TupacAmaru.Yacep.Expressions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.AtomicValue
{
    public class ParseIdentifierUnitTest
    {
        [Fact(DisplayName = "parse identifier expression")]
        public void ParseIdentifier()
        {
            var parser = new Core.Parser();

            var identifierExpression = parser.Parse("$aa") as IdentifierExpression;
            Assert.NotNull(identifierExpression);
            Assert.Equal("$aa", identifierExpression.Name);

            identifierExpression = parser.Parse("_11") as IdentifierExpression;
            Assert.NotNull(identifierExpression);
            Assert.Equal("_11", identifierExpression.Name);

            identifierExpression = parser.Parse("i") as IdentifierExpression;
            Assert.NotNull(identifierExpression);
            Assert.Equal("i", identifierExpression.Name);

            identifierExpression = parser.Parse("codemonk") as IdentifierExpression;
            Assert.NotNull(identifierExpression);
            Assert.Equal("codemonk", identifierExpression.Name);

            identifierExpression = parser.Parse("$a1a") as IdentifierExpression;
            Assert.NotNull(identifierExpression);
            Assert.Equal("$a1a", identifierExpression.Name);
        }
    }
}
