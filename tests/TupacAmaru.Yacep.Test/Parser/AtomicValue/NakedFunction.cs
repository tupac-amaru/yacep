using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.AtomicValue
{
    public class ParseNakedFunctionUnitTest
    {
        [Fact(DisplayName = "parse naked function expression")]
        public void ParseNakedFunction()
        {
            var option = ParseOption.CreateOption()
                .AddNakedFunction("abc", args => "abc")
                .AddNakedFunction("hash", args => args.GetHashCode())
                .AsReadOnly();

            var parser = new Core.Parser();

            var callExpression = parser.Parse("abc(1)", option) as NakedFunctionCallExpression;
            Assert.NotNull(callExpression);
            Assert.Equal("abc", callExpression.NakedFunction.Name);

            callExpression = parser.Parse("max(1,2,3)", option) as NakedFunctionCallExpression;
            Assert.NotNull(callExpression);
            Assert.Equal("max", callExpression.NakedFunction.Name);

            callExpression = parser.Parse("abc(hash(1,2,3))", option) as NakedFunctionCallExpression;
            Assert.NotNull(callExpression);
            Assert.Equal("abc", callExpression.NakedFunction.Name);
            callExpression = callExpression.Arguments[0] as NakedFunctionCallExpression;
            Assert.NotNull(callExpression);
            Assert.Equal("hash", callExpression.NakedFunction.Name);
        }
    }
}
