using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.Expression
{
    public class CompileBinaryExpressionUnitTest
    {
        [Fact(DisplayName = "compile or expression")]
        public void TestOr()
        {
            var compiler = Core.Compiler.Default;

            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();

            var trueValues = new[] { "1", "2.0", "true", "'aa'", "compiler" };
            var falseValues = new[] { "0", "-2.0", "false", "''", "null" };
            foreach (var item in trueValues)
            {
                foreach (var trueValue in trueValues)
                {
                    Assert.Equal(true, $"{item}||{trueValue}".Compile(option).Evaluate(null));
                }
                foreach (var falseValue in falseValues)
                {
                    Assert.Equal(true, $"{item}||{falseValue}".Compile(option).Evaluate(null));
                }
            }
            foreach (var item in falseValues)
            {
                foreach (var trueValue in trueValues)
                {
                    Assert.Equal(true, $"{item}||{trueValue}".Compile(option).Evaluate(null));
                }
                foreach (var falseValue in falseValues)
                {
                    Assert.Equal(false, $"{item}||{falseValue}".Compile(option).Evaluate(null));
                }
            }
        }

        [Fact(DisplayName = "compile and expression")]
        public void TestAnd()
        {
            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();

            var trueValues = new[] { "1", "2.0", "true", "'aa'", "compiler" };
            var falseValues = new[] { "0", "-2.0", "false", "''", "null" };
            foreach (var item in trueValues)
            {
                foreach (var trueValue in trueValues)
                {
                    Assert.Equal(true, $"{item}&&{trueValue}".Compile(option).Evaluate(null));
                }
                foreach (var falseValue in falseValues)
                {
                    Assert.Equal(false, $"{item}&&{falseValue}".Compile(option).Evaluate(null));
                }
            }
            foreach (var item in falseValues)
            {
                foreach (var trueValue in trueValues)
                {
                    Assert.Equal(false, $"{item}&&{trueValue}".Compile(option).Evaluate(null));
                }
                foreach (var falseValue in falseValues)
                {
                    Assert.Equal(false, $"{item}&&{falseValue}".Compile(option).Evaluate(null));
                }
            }
        }

        [Fact(DisplayName = "compile equal expression")]
        public void TestEqual()
        {
            Assert.Equal(false, "11==12".Compile().Evaluate(null));
            Assert.Equal(false, "11.7==12".Compile().Evaluate(null));
            Assert.Equal(false, "12.7==113".Compile().Evaluate(null));
            Assert.Equal(false, "12.7==13.1".Compile().Evaluate(null));
            Assert.Equal(false, "true==false".Compile().Evaluate(null));
            Assert.Equal(false, "'aa'=='ab'".Compile().Evaluate(null));
            Assert.Equal(false, "'aa'=='Ab'".Compile().Evaluate(null));

            Assert.Equal(true, "null==null".Compile().Evaluate(null));
            Assert.Equal(true, "12==12".Compile().Evaluate(null));
            Assert.Equal(true, "12==12.0".Compile().Evaluate(null));
            Assert.Equal(true, "12.0==12".Compile().Evaluate(null));
            Assert.Equal(true, "12.7==12.7".Compile().Evaluate(null));
            Assert.Equal(true, "true==true".Compile().Evaluate(null));
            Assert.Equal(true, "'aa'=='aa'".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler==134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'==134".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "true==134".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null==134".Compile().Evaluate(null));

            var array = new object[] { 'a', (sbyte)97, (byte)97, (ushort)97, (short)97, 97, (uint)97, 97L, (ulong)97, 97f, 97d, 97m };
            for (var i = 0; i < array.Length; i++)
            {
                for (var j = 0; j < array.Length; j++)
                {
                    var expr = $"this[{i}]==this[{j}]";
                    Assert.Equal(true, expr.Compile().Evaluate(array));
                }
            }
        }

        [Fact(DisplayName = "compile not equal expression")]
        public void TestNotEqual()
        {
            Assert.Equal(true, "11!=12".Compile().Evaluate(null));
            Assert.Equal(true, "11.7!=12".Compile().Evaluate(null));
            Assert.Equal(true, "12.7!=113".Compile().Evaluate(null));
            Assert.Equal(true, "12.7!=13.1".Compile().Evaluate(null));
            Assert.Equal(true, "true!=false".Compile().Evaluate(null));
            Assert.Equal(true, "'aa'!='ab'".Compile().Evaluate(null));
            Assert.Equal(true, "'aa'!='Ab'".Compile().Evaluate(null));

            Assert.Equal(false, "null!=null".Compile().Evaluate(null));
            Assert.Equal(false, "12!=12".Compile().Evaluate(null));
            Assert.Equal(false, "12!=12.0".Compile().Evaluate(null));
            Assert.Equal(false, "12.0!=12".Compile().Evaluate(null));
            Assert.Equal(false, "12.7!=12.7".Compile().Evaluate(null));
            Assert.Equal(false, "true!=true".Compile().Evaluate(null));
            Assert.Equal(false, "'aa'!='aa'".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler!=134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'!=134".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "true!=134".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null!=134".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile less expression")]
        public void TestLess()
        {
            Assert.Equal(true, "11<12".Compile().Evaluate(null));
            Assert.Equal(true, "11.7<12".Compile().Evaluate(null));
            Assert.Equal(true, "12.7<13".Compile().Evaluate(null));
            Assert.Equal(true, "12.7<13.1".Compile().Evaluate(null));

            Assert.Equal(false, "12<12".Compile().Evaluate(null));
            Assert.Equal(false, "12<12.0".Compile().Evaluate(null));
            Assert.Equal(false, "12.0<12".Compile().Evaluate(null));
            Assert.Equal(false, "12.7<12.7".Compile().Evaluate(null));

            Assert.Equal(false, "12<11".Compile().Evaluate(null));
            Assert.Equal(false, "12<11.0".Compile().Evaluate(null));
            Assert.Equal(false, "12.7<11".Compile().Evaluate(null));
            Assert.Equal(false, "12.7<11.7".Compile().Evaluate(null));


            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler<134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'<34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "true<34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null<34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34<null".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile greater expression")]
        public void TestGreater()
        {
            Assert.Equal(true, "12>11".Compile().Evaluate(null));
            Assert.Equal(true, "12>11.7".Compile().Evaluate(null));
            Assert.Equal(true, "12.7>11".Compile().Evaluate(null));
            Assert.Equal(true, "12.7>11.1".Compile().Evaluate(null));


            Assert.Equal(false, "12>12".Compile().Evaluate(null));
            Assert.Equal(false, "12>12.0".Compile().Evaluate(null));
            Assert.Equal(false, "12.0>12".Compile().Evaluate(null));
            Assert.Equal(false, "12.7>12.7".Compile().Evaluate(null));

            Assert.Equal(false, "11>12".Compile().Evaluate(null));
            Assert.Equal(false, "11.7>12".Compile().Evaluate(null));
            Assert.Equal(false, "12.7>13".Compile().Evaluate(null));
            Assert.Equal(false, "12.7>13.1".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler>134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'>34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "true>34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null>34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34>null".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile less equal expression")]
        public void TestLessEqual()
        {
            Assert.Equal(true, "11<=12".Compile().Evaluate(null));
            Assert.Equal(true, "11.7<=12".Compile().Evaluate(null));
            Assert.Equal(true, "12.7<=13".Compile().Evaluate(null));
            Assert.Equal(true, "12.7<=13.1".Compile().Evaluate(null));

            Assert.Equal(true, "12<=12".Compile().Evaluate(null));
            Assert.Equal(true, "12<=12.0".Compile().Evaluate(null));
            Assert.Equal(true, "12.0<=12".Compile().Evaluate(null));
            Assert.Equal(true, "12.7<=12.7".Compile().Evaluate(null));

            Assert.Equal(false, "12<=11".Compile().Evaluate(null));
            Assert.Equal(false, "12<=11.0".Compile().Evaluate(null));
            Assert.Equal(false, "12.7<=11".Compile().Evaluate(null));
            Assert.Equal(false, "12.7<=11.7".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler<=134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'<=34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "true<=34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null<=34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34<=null".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile greater equal expression")]
        public void TestGreaterEqual()
        {
            Assert.Equal(true, "12>=11".Compile().Evaluate(null));
            Assert.Equal(true, "12>=11.7".Compile().Evaluate(null));
            Assert.Equal(true, "12.7>=11".Compile().Evaluate(null));
            Assert.Equal(true, "12.7>=11.1".Compile().Evaluate(null));


            Assert.Equal(true, "12>=12".Compile().Evaluate(null));
            Assert.Equal(true, "12>=12.0".Compile().Evaluate(null));
            Assert.Equal(true, "12.0>=12".Compile().Evaluate(null));
            Assert.Equal(true, "12.7>=12.7".Compile().Evaluate(null));

            Assert.Equal(false, "11>=12".Compile().Evaluate(null));
            Assert.Equal(false, "11.7>=12".Compile().Evaluate(null));
            Assert.Equal(false, "12.7>=13".Compile().Evaluate(null));
            Assert.Equal(false, "12.7>=13.1".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler>=134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'>=34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "true>=34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null>=34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34>=null".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile right add expression")]
        public void TestAdd()
        {
            Assert.Equal(46, "12+34".Compile().Evaluate(null));
            Assert.Equal(888888888 + 12, "12+888888888".Compile().Evaluate(null));
            Assert.Equal(8888888888 + 12UL, "12+8888888888".Compile().Evaluate(null));
            Assert.Equal(24.7m, "12+12.7".Compile().Evaluate(null));
            Assert.Equal(46.7m, "12.7+34".Compile().Evaluate(null));
            Assert.Equal(25.1m, "12.4+12.7".Compile().Evaluate(null));
            Assert.Equal("abc", "'a'+'bc'".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler+134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "true+34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'a'+34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34+null".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null+34".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile minus expression")]
        public void TestMinus()
        {
            Assert.Equal(1, "12-11".Compile().Evaluate(null));
            Assert.Equal(.9m, "12-11.1".Compile().Evaluate(null));
            Assert.Equal(1.8009m, "12.8009-11".Compile().Evaluate(null));
            Assert.Equal(1.0009m, "12.8009-11.8".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler-134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "false-34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'a'-34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34-null".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null-34".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile multiply expression")]
        public void TestMultiply()
        {
            Assert.Equal(12 * 11, "12*11".Compile().Evaluate(null));
            Assert.Equal(133.2m, "12*11.1".Compile().Evaluate(null));
            Assert.Equal(140.8099m, "12.8009*11".Compile().Evaluate(null));
            Assert.Equal(151.05062m, "12.8009*11.8".Compile().Evaluate(null));
            Assert.Equal("ababab", "3*'ab'".Compile().Evaluate(null));
            Assert.Equal("ababab", "'ab'*3".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler*134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null*34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "false*34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34*null".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null*34".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile divide expression")]
        public void TestDivide()
        {
            Assert.Equal(2, "22/11".Compile().Evaluate(null));
            Assert.Equal(12.5m, "25/2.0".Compile().Evaluate(null));
            Assert.Equal(14.111m, "28.222/2".Compile().Evaluate(null));
            Assert.Equal(12.07m, "36.21/3.0".Compile().Evaluate(null));

            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler/134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'/34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null/34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "false/34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34/null".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null/34".Compile().Evaluate(null));
        }

        [Fact(DisplayName = "compile modulo expression")]
        public void TestModulo()
        {
            Assert.Equal(0, "22%2".Compile().Evaluate(null));
            Assert.Equal(1m, "25%2.0".Compile().Evaluate(null));
            Assert.Equal(0.222m, "28.222%2".Compile().Evaluate(null));
            Assert.Equal(0.21m, "36.21%3.0".Compile().Evaluate(null));
             
            var compiler = Core.Compiler.Default;
            var option = ParseOption.CreateOption()
                .AddLiteralValue("compiler", compiler)
                .AsReadOnly();
            Assert.Throws<UnsupportedOperationException>(() => "compiler%134".Compile(option).Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "'aa'%34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "false%34".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "34%null".Compile().Evaluate(null));
            Assert.Throws<UnsupportedOperationException>(() => "null%34".Compile().Evaluate(null));
        }
    }
}