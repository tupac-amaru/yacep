using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.Expression
{
    public class CompileUnaryExpressionUnitTest
    {
        [Fact(DisplayName = "compile negative expression")]
        public void TestNegative()
        {
            Assert.Equal(-12, "-12".Compile().Evaluate(null));
            Assert.Equal(-12.8m, "-12.8".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "-compiler".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "-'aa'".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "-true".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "-null".Compile().Evaluate(null));

            var array = new object[] { 'a', (sbyte)-1, (byte)2, (ushort)3, (short)-3, -4, (uint)4, -5L, 6.0f, 7.0222d, 8.7232718m };
            Assert.Equal(-97, compiler.Compile("-this[0]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(1, compiler.Compile("-this[1]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(-2, compiler.Compile("-this[2]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(-3, compiler.Compile("-this[3]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(3, compiler.Compile("-this[4]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(4, compiler.Compile("-this[5]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(-4L, compiler.Compile("-this[6]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(5L, compiler.Compile("-this[7]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(-6.0f, compiler.Compile("-this[8]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(-7.0222d, compiler.Compile("-this[9]".ToEvaluableExpression()).Evaluate(array));
            Assert.Equal(-8.7232718m, compiler.Compile("-this[10]".ToEvaluableExpression()).Evaluate(array));
            Assert.Throws<UnsupportedOperationException>(() => compiler.Compile("-a".ToEvaluableExpression()).Evaluate(new { a = (ulong)222 }));
        }

        [Fact(DisplayName = "compile positive expression")]
        public void TestPositive()
        {
            var compiler = Core.Compiler.Default;
            Assert.Equal(12, compiler.Compile("+12".ToEvaluableExpression()).Evaluate(null));
            Assert.Equal(12.8m, compiler.Compile("+12.8".ToEvaluableExpression()).Evaluate(null));

            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => compiler.Compile("+compiler".ToEvaluableExpression(option)).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => compiler.Compile("+'aa'".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => compiler.Compile("+true".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => compiler.Compile("+null".ToEvaluableExpression()).Evaluate(null));

            var array = new object[] { 'a', (sbyte)-1, (byte)2, (ushort)3, (short)-3, -4, (uint)4, -5L, (ulong)222, 6.0f, 7.0222d, 8.7232718m };
            for (var i = 0; i < array.Length; i++)
            {
                var value = array[i];
                var expr = $"+this[{i}]";
                Assert.Equal(value, compiler.Compile(expr.ToEvaluableExpression()).Evaluate(array));
            }
        }

        [Fact(DisplayName = "compile not expression")]
        public void TestNot()
        {
            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();

            foreach (var item in new[] { "1", "2.0", "true", "'aa'", "compiler" })
                Assert.Equal(false, compiler.Compile($"!{item}".ToEvaluableExpression(option)).Evaluate(null));
            foreach (var item in new[] { "0", "-2.0", "false", "''", "null" })
                Assert.Equal(true, compiler.Compile($"!{item}".ToEvaluableExpression(option)).Evaluate(null));
            var array = new object[] { 'a', (sbyte)1, (byte)2, (ushort)3, (short)5, 66, (uint)4, 88L, (ulong)222, 6.0f, 7.0222d, 8.7232718m };
            for (var i = 0; i < array.Length; i++)
            {
                var expr = $"!this[{i}]";
                Assert.Equal(false, compiler.Compile(expr.ToEvaluableExpression()).Evaluate(array));
            }
            array = new object[] { char.MinValue, sbyte.MinValue, (byte)0, (ushort)0, (short)-5, -66, (uint)0, long.MinValue, (ulong)0, -6.0f, -7.0222d, -8.7232718m };
            for (var i = 0; i < array.Length; i++)
            {
                var expr = $"!this[{i}]";
                Assert.Equal(true, compiler.Compile(expr.ToEvaluableExpression()).Evaluate(array));
            }
        }

        [Fact(DisplayName = "compile not expression")]
        public void TestCustomUnaryOperator()
        {
            var parser = new Core.Parser();
            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AddUnaryOperator("@", s => s != null)
                .AddUnaryOperator("not", s => s != null).AsReadOnly();

            foreach (var item in new[] { "1", "2.0", "true", "'aa'", "compiler" })
                Assert.Equal(false, compiler.Compile($"!{item}".ToEvaluableExpression(option)).Evaluate(null));
            foreach (var item in new[] { "0", "-2.0", "false", "''", "null" })
                Assert.Equal(true, compiler.Compile($"!{item}".ToEvaluableExpression(option)).Evaluate(null));
            var array = new object[] { 'a', (sbyte)1, (byte)2, (ushort)3, (short)5, 66, (uint)4, 88L, (ulong)222, 6.0f, 7.0222d, 8.7232718m };
            for (var i = 0; i < array.Length; i++)
            {
                var expr = $"!this[{i}]";
                Assert.Equal(false, compiler.Compile(expr.ToEvaluableExpression()).Evaluate(array));
                Assert.Equal(true, $"@this[{i}]".Compile(option).Evaluate(array));
                Assert.Equal(true, $"not(this[{i}])".Compile(option).Evaluate(array));
            }
            array = new object[] { char.MinValue, sbyte.MinValue, (byte)0, (ushort)0, (short)-5, -66, (uint)0, long.MinValue, (ulong)0, -6.0f, -7.0222d, -8.7232718m };
            for (var i = 0; i < array.Length; i++)
            {
                var expr = $"!this[{i}]";
                Assert.Equal(true, expr.Compile(parser, compiler).Evaluate(array));
            }
        }
    }
}
