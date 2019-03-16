using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.AtomicValue
{
    public class CompileLiteralUnitTest
    {
        [Fact(DisplayName = "compile literal expression")]
        public void CompileLiteral()
        {
            var array = new object[] { 'a', (sbyte)-1, (byte)2, (ushort)3, (short)-3, -4, (uint)4, -5L, (ulong)5, 6.0f, 7.0222d, 8.7232718m };
            var option = ParseOption.CreateOption()
                .AddLiteralValue("array", array)
                .AddLiteralValue("abc", -90.44m)
                .AsReadOnly();

            Assert.Equal(array.Length, "len(array)".Compile(option).Evaluate(null));
            Assert.Equal(array.Length, "array.Length".Compile(option).Evaluate(null));
            Assert.Equal(97m, "max(array)".Compile(option).Evaluate(null));
            Assert.Equal(-5m, "min(array)".Compile(option).Evaluate(null));
            Assert.Equal(119.7454718m, "sum(array)".Compile(option).Evaluate(null));
            Assert.Equal(-90.44m, "abc".Compile(option).Evaluate(null));
            Assert.Null("null".Compile().Evaluate(null));
            Assert.Null("null".Compile().Evaluate(12));
            Assert.Null("null".Compile().EvaluateAs<object>());
            Assert.Null("null".Compile().EvaluateAsType(typeof(object)));
            Assert.True("true".Compile().EvaluateAs<bool>());
            Assert.False("false".Compile().EvaluateAs<bool>());


            option = ParseOption.CreateOption()
                .AddLiteralValue("longValue1", 2147483649L)
                .AddLiteralValue("longValue2", 2147483641L)
                .AsReadOnly();
            Assert.Equal(2147483649L, "longValue1".Compile(option).EvaluateAs<long>());
            Assert.Equal(2147483641, "longValue2".Compile(option).EvaluateAs<int>());
            Assert.Equal(2147483647, "2147483647".Compile().EvaluateAs<int>());
            Assert.Equal(2147483646, "2147483646".Compile().EvaluateAs<int>());
            Assert.Equal(4294967295, "4294967295".Compile().EvaluateAs<uint>());
            Assert.Equal(4294967294, "4294967294".Compile().EvaluateAs<uint>());
        }
    }
}
