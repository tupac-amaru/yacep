using TupacAmaru.Yacep.Expressions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.Expression
{
    public class ParseUnaryExpressionUnitTest
    {
        [Fact(DisplayName = "parse unary expression")]
        public void ParseUnaryExpression()
        {
            var parser = new Core.Parser();

            var unaryExpression = parser.Parse("+12") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("+", unaryExpression.UnaryOperator.Operator);
            var constantExpression = unaryExpression.Argument as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("12", constantExpression.Raw);
            Assert.Equal(12, constantExpression.Value);

            unaryExpression = parser.Parse("-19.0") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("-", unaryExpression.UnaryOperator.Operator);
            constantExpression = unaryExpression.Argument as ConstantExpression;
            Assert.NotNull(constantExpression);
            Assert.Equal("19.0", constantExpression.Raw);
            Assert.Equal(19.0m, constantExpression.Value);

            unaryExpression = parser.Parse("!true") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("!", unaryExpression.UnaryOperator.Operator);
            var literalExpression = unaryExpression.Argument as LiteralExpression;
            Assert.NotNull(literalExpression);
            Assert.True((bool)literalExpression.LiteralValue.Value);

            unaryExpression = parser.Parse("!$11") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("!", unaryExpression.UnaryOperator.Operator);
            var identifier = unaryExpression.Argument as IdentifierExpression;
            Assert.NotNull(identifier);
            Assert.Equal("$11", identifier.Name);

            unaryExpression = parser.Parse("!_11") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("!", unaryExpression.UnaryOperator.Operator);
            identifier = unaryExpression.Argument as IdentifierExpression;
            Assert.NotNull(identifier);
            Assert.Equal("_11", identifier.Name);

            unaryExpression = parser.Parse("!cm") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("!", unaryExpression.UnaryOperator.Operator);
            identifier = unaryExpression.Argument as IdentifierExpression;
            Assert.NotNull(identifier);
            Assert.Equal("cm", identifier.Name);


            unaryExpression = parser.Parse("!(12+34)") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("!", unaryExpression.UnaryOperator.Operator);
            var binaryExpression = unaryExpression.Argument as BinaryExpression;
            Assert.NotNull(binaryExpression);
            Assert.Equal("+", binaryExpression.BinaryOperator.Operator);
            var left = binaryExpression.Left as ConstantExpression;
            Assert.NotNull(left);
            Assert.Equal(12, left.Value);
            var right = binaryExpression.Right as ConstantExpression;
            Assert.NotNull(right);
            Assert.Equal(34, right.Value);

            unaryExpression = parser.Parse("+avg(a,abc)") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("+", unaryExpression.UnaryOperator.Operator);
            var callExpression = unaryExpression.Argument as NakedFunctionCallExpression;
            Assert.NotNull(callExpression);
            Assert.Equal("avg", callExpression.NakedFunction.Name);
            var a = callExpression.Arguments[0] as IdentifierExpression;
            Assert.NotNull(a);
            Assert.Equal("a", a.Name);
            var b = callExpression.Arguments[1] as IdentifierExpression;
            Assert.NotNull(b);
            Assert.Equal("abc", b.Name);

            unaryExpression = parser.Parse("![1,'asa',true]") as UnaryExpression;
            Assert.NotNull(unaryExpression);
            Assert.Equal("!", unaryExpression.UnaryOperator.Operator);

            var arrayExpression = unaryExpression.Argument as ArrayExpression;
            Assert.NotNull(arrayExpression);
            Assert.Equal(3, arrayExpression.Elements.Length);
        }
    }
}
