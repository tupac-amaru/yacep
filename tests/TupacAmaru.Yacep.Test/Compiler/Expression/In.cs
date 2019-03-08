using System;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.Expression
{
    public class CompileInExpressionUnitTest
    {
        [Fact(DisplayName = "compile in expression")]
        public void CompileInExpression()
        {
            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();

            Assert.Throws<ArgumentNullException>(() => compiler.Compile(null));

            var evaluator = "1 in (1, 2.0222 ,true,'das', false, null, compiler)".Compile(option);
            Assert.Equal(true, evaluator.Evaluate(null));

            evaluator = "2.0222 in (1, 2.0222 ,true,'das', false, null, compiler)".Compile(option);
            Assert.Equal(true, evaluator.Evaluate(null));

            evaluator = "true in (1, 2.0222 ,true, 'das', false, null, compiler)".Compile(option);
            Assert.Equal(true, evaluator.Evaluate(null));

            evaluator = "'das' in (1, 2.0222 ,true,'das', false, null, compiler)".Compile(option);
            Assert.Equal(true, evaluator.Evaluate(null));

            evaluator = "false in (1, 2.0222 ,true,'das', false, null, compiler)".Compile(option);
            Assert.Equal(true, evaluator.Evaluate(null));

            evaluator = "null in (1, 2.0222 ,true,'das', false, null, compiler)".Compile(option);
            Assert.Equal(true, evaluator.Evaluate(null));

            evaluator = "compiler in (1, 2.0222 ,true,'das', false, null, compiler)".Compile(option);
            Assert.Equal(true, evaluator.Evaluate(null));

            evaluator = "99 in (1, 2.0222 ,true,'das', false, null, compiler)".Compile(option);
            Assert.Equal(false, evaluator.Evaluate(null));
        }
    }
}
