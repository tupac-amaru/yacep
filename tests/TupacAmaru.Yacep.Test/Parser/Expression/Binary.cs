using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.Expression
{
    public class ParseBinaryExpressionUnitTest
    {
        [Fact(DisplayName = "parse binary expression")]
        public void ParseBinaryExpression()
        {
            var parser = new Core.Parser();

            var binaryExpression = parser.Parse("12+34+true") as BinaryExpression;
            Assert.NotNull(binaryExpression);
            Assert.Equal("+", binaryExpression.BinaryOperator.Operator);
            Assert.Equal(true, (binaryExpression.Right as LiteralExpression)?.LiteralValue.Value);

            Assert.Equal(0, binaryExpression.StartIndex);
            Assert.Equal(10, binaryExpression.EndIndex);

            var left = binaryExpression.Left as BinaryExpression;
            Assert.NotNull(left);
            Assert.Equal("+", left.BinaryOperator.Operator);
            Assert.Equal(12, (left.Left as ConstantExpression)?.Value);
            Assert.Equal(34, (left.Right as ConstantExpression)?.Value);

            binaryExpression = parser.Parse("12*34+true") as BinaryExpression;
            Assert.NotNull(binaryExpression);
            Assert.Equal("+", binaryExpression.BinaryOperator.Operator);
            Assert.Equal(true, (binaryExpression.Right as LiteralExpression)?.LiteralValue.Value);

            left = binaryExpression.Left as BinaryExpression;
            Assert.NotNull(left);
            Assert.Equal("*", left.BinaryOperator.Operator);
            Assert.Equal(12, (left.Left as ConstantExpression)?.Value);
            Assert.Equal(34, (left.Right as ConstantExpression)?.Value);

            binaryExpression = parser.Parse("(12*34+true)||(false)") as BinaryExpression;
            Assert.NotNull(binaryExpression);
            Assert.Equal("||", binaryExpression.BinaryOperator.Operator);
            Assert.Equal(false, (binaryExpression.Right as LiteralExpression)?.LiteralValue.Value);

            left = binaryExpression.Left as BinaryExpression;
            Assert.NotNull(left);
            Assert.Equal("+", left.BinaryOperator.Operator);
            Assert.Equal(true, (left.Right as LiteralExpression)?.LiteralValue.Value);

            var right = left.Left as BinaryExpression;
            Assert.NotNull(right);
            Assert.Equal("*", right.BinaryOperator.Operator);
            Assert.Equal(12, (right.Left as ConstantExpression)?.Value);
            Assert.Equal(34, (right.Right as ConstantExpression)?.Value);


            binaryExpression = parser.Parse("12 + 19") as BinaryExpression;
            Assert.NotNull(binaryExpression);
            Assert.Equal("+", binaryExpression.BinaryOperator.Operator);
            Assert.Equal(12, (binaryExpression.Left as ConstantExpression)?.Value);
            Assert.Equal(19, (binaryExpression.Right as ConstantExpression)?.Value);

            var option = ParseOption.CreateOption()
                .AddBinaryOperator("@", (a, b) => null, 8)
                .AsReadOnly();
            binaryExpression = parser.Parse("12 @ true", option) as BinaryExpression;
            Assert.NotNull(binaryExpression);
            Assert.Equal("@", binaryExpression.BinaryOperator.Operator);
            Assert.Equal(12, (binaryExpression.Left as ConstantExpression)?.Value);
            Assert.Equal(true, (binaryExpression.Right as LiteralExpression)?.LiteralValue.Value);

            option = ParseOption.CreateOption()
                 .AddBinaryOperator("add", (a, b) => null, 8)
                 .AsReadOnly();
            var add = parser.Parse("12 add 34", option) as BinaryExpression;
            Assert.NotNull(add);
            Assert.Equal("add", add.BinaryOperator.Operator);
            Assert.Equal(12, (add.Left as ConstantExpression)?.Value);
            Assert.Equal(34, (add.Right as ConstantExpression)?.Value);
        }
    }
}
