using System;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.CompoundValue
{
    public class CompileArrayExpressionUnitTest
    {
        [Fact(DisplayName = "compile array expression")]
        public void CompileArray()
        {
            var compiler = Core.Compiler.Default;

            Assert.Throws<ArgumentNullException>(() => compiler.Compile(null));

            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();

            var evaluator = "[1, 2.0222, -9.7777, true,'das', false, null, compiler]".Compile(option);
            var array = (object[])evaluator.Evaluate(null);
            Assert.Equal(8, array.Length);
            Assert.Equal(1, array[0]);
            Assert.Equal(2.0222m, array[1]);
            Assert.Equal(-9.7777m, array[2]);
            Assert.Equal(true, array[3]);
            Assert.Equal("das", array[4]);
            Assert.Equal(false, array[5]);
            Assert.Null(array[6]);
            Assert.Equal(compiler, array[7]);

            Assert.Equal(2, "this[1]".Compile().EvaluateAs<int>(new object[] { 1, 2, 3 }));
        }
    }
}
