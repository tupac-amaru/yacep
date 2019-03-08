using System;
using System.Linq;
using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using TupacAmaru.Yacep.Symbols;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.Expression
{
    public class CompileConditionalExpressionUnitTest
    {
        [Fact(DisplayName = "compile conditional expression")]
        public void CompileConditionalExpression()
        {
            var parser = new Core.Parser();
            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();

            var trueValues = new[] { "1", "2.0", "true", "'aa'", "compiler" };
            var falseValues = new[] { "0", "-2.0", "false", "''", "null" };

            foreach (var item in trueValues)
            {
                var expr = $"{item}?true:false";
                Assert.Equal(true, expr.Compile(parser, option).Evaluate(null));
            }
            foreach (var item in falseValues)
            {
                var expr = $"{item}?true:false";
                Assert.Equal(false, expr.Compile(parser, option, compiler).Evaluate(null));
            }
        }
    }
}
