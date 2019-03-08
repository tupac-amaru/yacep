using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.AtomicValue
{
    public class CompileDecimalUnitTest
    {
        [Fact(DisplayName = "compile decimal expression")]
        public void CompileDecimal()
        {
            Assert.Equal(3.155225M, "3.155225".Compile().EvaluateAs<decimal>());
            Assert.Equal(10.0M, "10.0".Compile().EvaluateAs<decimal>());
            Assert.Equal(.01212M, ".01212".Compile().EvaluateAs<decimal>());
            Assert.Equal(.01212M, ".01212".Compile().EvaluateAs<decimal>());
            Assert.Equal(.01212M, ".01212".Compile().EvaluateAs<decimal>());
            Assert.Equal(1.3e-05M, "1.3e-5".Compile().EvaluateAs<decimal>());
            Assert.Equal(238800M, "23.88e4".Compile().EvaluateAs<decimal>());
            Assert.Equal(124500M, "12.45E4".Compile().EvaluateAs<decimal>());
            Assert.Equal(124500M, "12.45E4".Compile().EvaluateAs<decimal>());
            Assert.Equal(100000M, "100e+3".Compile().EvaluateAs<decimal>());
            Assert.Equal(20000M, "20e3".Compile().EvaluateAs<decimal>());
            Assert.Equal(120_000M, "12E4".Compile().EvaluateAs<decimal>());
        }
    }
}
