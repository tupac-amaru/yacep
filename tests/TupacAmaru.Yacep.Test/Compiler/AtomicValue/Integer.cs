using System;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.AtomicValue
{
    public class CompileIntegerUnitTest
    {
        [Fact(DisplayName = "compile string expression")]
        public void CompileInteger()
        {
            Assert.Equal(100, "100".Compile().EvaluateAs<int>());
            Assert.Equal(12312381, "12312381".Compile().EvaluateAs<int>());
            Assert.Equal(956869218, "956869218".Compile().EvaluateAs<int>());
            Assert.Equal(2147483649, "2147483649".Compile().EvaluateAs<uint>());
            Assert.Equal(98147483649UL, "98147483649".Compile().EvaluateAs<ulong>());
            Assert.Equal(StringSplitOptions.RemoveEmptyEntries, "'RemoveEmptyEntries'".Compile().EvaluateAs<StringSplitOptions>());

            Assert.Throws<FormatException>(() => "'12'".Compile().EvaluateAs<Fixture>());
            Assert.Throws<FormatException>(() => "'a'".Compile().EvaluateAs<int>());
            Assert.Throws<FormatException>(() => "1212".Compile().EvaluateAs<StringSplitOptions>());
        }
    }
}
