using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.AtomicValue
{
    public class ParseIntegerUnitTest
    {
        [Fact(DisplayName = "parse integer expression")]
        public void ParseInteger()
        {
            var parser = new Core.Parser();

            var constantExpression = parser.Parse("100") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("100", constantExpression.Raw);
            Assert.Equal(100, constantExpression.Value);

            constantExpression = parser.Parse("12312381") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("12312381", constantExpression.Raw);
            Assert.Equal(12312381, constantExpression.Value);

            constantExpression = parser.Parse("956869218") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("956869218", constantExpression.Raw);
            Assert.Equal(956869218, constantExpression.Value);

            var option = ParseOption.CreateOption()
                .NotAllowedConvertUnsignedInteger()
                .AsReadOnly();

            constantExpression = parser.Parse("956869218", option) as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("956869218", constantExpression.Raw);
            Assert.NotEqual(956869218, constantExpression.Value);
            Assert.Equal(956869218UL, constantExpression.Value);
        }
    }
}
