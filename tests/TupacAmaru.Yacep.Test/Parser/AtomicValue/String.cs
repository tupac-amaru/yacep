using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.AtomicValue
{
    public class ParseStringUnitTest
    {
        [Fact(DisplayName = "parse string expression")]
        public void ParseString()
        {
            var parser = new Core.Parser();

            var constantExpression = parser.Parse("'abasdkas'") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("abasdkas", constantExpression.Raw);
            Assert.Equal("abasdkas", constantExpression.Value);

            constantExpression = "'abasdkas'".ToEvaluableExpression() as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("abasdkas", constantExpression.Raw);
            Assert.Equal("abasdkas", constantExpression.Value);

            constantExpression = "'abasdkas'".ToEvaluableExpression(parser) as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("abasdkas", constantExpression.Raw);
            Assert.Equal("abasdkas", constantExpression.Value);

            constantExpression = parser.Parse("\"as1781uhj21\"") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("as1781uhj21", constantExpression.Raw);
            Assert.Equal("as1781uhj21", constantExpression.Value);

            constantExpression = parser.Parse("\"121asadas\t1uhj21\"") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("121asadas\t1uhj21", constantExpression.Raw);
            Assert.Equal("121asadas\t1uhj21", constantExpression.Value);

            constantExpression = parser.Parse("'twew\\tqqwkqekkk'") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("twew\tqqwkqekkk", constantExpression.Raw);
            Assert.Equal("twew\tqqwkqekkk", constantExpression.Value);


            constantExpression = parser.Parse("'aksja\\n6127\\t1makd\\bahjkla;+1212/1212\\r\\f\\v'") as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("aksja\n6127\t1makd\bahjkla;+1212/1212\r\f\v", constantExpression.Raw);
            Assert.Equal("aksja\n6127\t1makd\bahjkla;+1212/1212\r\f\v", constantExpression.Value);
        }
    }
}
