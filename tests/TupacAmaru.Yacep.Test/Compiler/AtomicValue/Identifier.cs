using System;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.AtomicValue
{
    public class CompileIdentifierUnitTest
    {
        [Fact(DisplayName = "compile identifier expression")]
        public void CompileIdentifier()
        {
            var array = new object[] { 'a', (sbyte)-1, (byte)2, (ushort)3, (short)-3, -4, (uint)4, -5L, (ulong)5, 6.0f, 7.0222d, 8.7232718m };
            Assert.Equal(array.Length, "len(this)".Compile().Evaluate(array));
            Assert.Equal(119.7454718m, "sum(this)".Compile().Evaluate(array));

            array = new object[] { -5L, -4, (short)-3, (sbyte)-1, (byte)2, (ushort)3, (uint)4, (ulong)5, 6.0f, 7.0222d, 8.7232718m, 'a' };
            Assert.Equal(97m, "max(this)".Compile().Evaluate(array));
            Array.Reverse(array);
            Assert.Equal(-5m, "min(this)".Compile().Evaluate(array));

            Assert.Equal(-12m, "t".Compile().EvaluateAs<decimal>(new { t = -12f }));
            Assert.Equal(-12m, "t".Compile().EvaluateAsType(new { t = -12f }, typeof(decimal)));
            Assert.Equal("-12", "t".Compile().EvaluateAs<string>(new { t = -12f }));
            Assert.Equal("-12", "t".Compile().EvaluateAsType(new { t = -12f }, typeof(string)));
        }
    }
}
