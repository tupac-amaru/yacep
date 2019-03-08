using TupacAmaru.Yacep.Expressions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.AtomicValue
{
    public class ParseDecimalUnitTest
    { 
        [Fact(DisplayName = "parse decimal expression")]
        public void ParseDecimal()
        {
            var parser = new Core.Parser();

            var constantExpression = parser.Parse("3.155225") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("3.155225", constantExpression.Raw);
            Assert.Equal(3.155225M, constantExpression.Value);

            constantExpression = parser.Parse("10.0") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("10.0", constantExpression.Raw);
            Assert.Equal(10.0M, constantExpression.Value);

            constantExpression = parser.Parse(".01212") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal(".01212", constantExpression.Raw);
            Assert.Equal(.01212M, constantExpression.Value);

            constantExpression = parser.Parse("1.3e-5") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("1.3e-5", constantExpression.Raw);
            Assert.Equal(1.3e-05M, constantExpression.Value);

            constantExpression = parser.Parse("23.88e4") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("23.88e4", constantExpression.Raw);
            Assert.Equal(238800M, constantExpression.Value);

            constantExpression = parser.Parse("12.45E4") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("12.45E4", constantExpression.Raw);
            Assert.Equal(124500M, constantExpression.Value);

            constantExpression = parser.Parse("100e+3") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("100e+3", constantExpression.Raw);
            Assert.Equal(100000M, constantExpression.Value);

            constantExpression = parser.Parse("20e3") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("20e3", constantExpression.Raw);
            Assert.Equal(20000M, constantExpression.Value);

            constantExpression = parser.Parse("12E4") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("12E4", constantExpression.Raw);
            Assert.Equal(120_000M, constantExpression.Value);
        }
    }
}
