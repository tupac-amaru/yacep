using System;
using System.Diagnostics;
using System.Linq;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;
using TupacAmaru.Yacep.Symbols;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.AtomicValue
{
    public class CompileNakedFunctionUnitTest
    {
        [Fact(DisplayName = "compile custom nake function expression")]
        public void CompileCustomFunction()
        {
            var option = ParseOption.CreateOption()
                .AddNakedFunction("tell_me_the_cat_name", s => "tom", true)
                .AddNakedFunction("tell_me_the_mouse_name", s => "jerry", true)
                .AddBinaryOperator("and", (a, b) => $"{a} and {b}", 10)
                .AddNakedFunction("answer_for_question", s => (s[0] as string).Replace("吗", "").Replace("?", "!"))
                .AddBinaryOperator("is", (a, b) => string.Equals(a as string, b as string), 10)
                .AddLiteralValue("cat", "cat")
                .AddLiteralValue("mouse", "mouse")
                .AddNakedFunction("tell_me_the_name_of", s =>
                {
                    switch (s[0])
                    {
                        case "cat":
                            return "tom";
                        case "mouse":
                            return "jerry";
                        default:
                            return "i don't know";
                    }
                }, true)
                .AsReadOnly();
            Assert.Equal("tom and jerry", "tell_me_the_cat_name() and tell_me_the_mouse_name()".Compile(option).EvaluateAs<string>());
            Assert.Equal("tom and jerry", "tell_me_the_name_of(cat) and tell_me_the_name_of(mouse)".Compile(option).EvaluateAs<string>());
            Assert.True("answer_for_question('在吗?') is '在!'".Compile(option).EvaluateAs<bool>());
            Assert.True("answer_for_question('你好') is '你好'".Compile(option).EvaluateAs<bool>());
            Assert.True("answer_for_question('能听懂汉语吗?') is '能听懂汉语!'".Compile(option).EvaluateAs<bool>());
            Assert.True("answer_for_question('真的吗?') is '真的!'".Compile(option).EvaluateAs<bool>());
        }

        [Fact(DisplayName = "compile statistics function expression")]
        public void CompileNakeStatisticsFunctions()
        {
            var compiler = new Core.Compiler();

            Assert.Throws<ArgumentNullException>(() => compiler.Compile(null));
            var option = ParseOption.CreateOption();
            option.LiteralValues.Add(new LiteralValue("compiler", compiler));

            Assert.Equal(4, compiler.Compile("len([1, 2.0222, true, 'das'])".ToEvaluableExpression()).Evaluate(null));
            Assert.Equal(0, compiler.Compile("len([])".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("len()".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("len(1,8)".ToEvaluableExpression()).Evaluate(null));

            Assert.Equal(11.5m, compiler.Compile("max([1, -2.0222, -89, 11.5])".ToEvaluableExpression()).Evaluate(null));
            Assert.Equal(110m, compiler.Compile("max([1, 110, -89, 100])".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("max()".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("max(1,8)".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("max([1, 2.0222 ,true,'das', false, null, compiler])".ToEvaluableExpression(option.AsReadOnly())).Evaluate(null));

            Assert.Equal(-89m, compiler.Compile("min([1, -2.0222, -89, 11.5])".ToEvaluableExpression()).Evaluate(null));
            Assert.Equal(-90.1m, compiler.Compile("min([1, 110, -90.1, 100])".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("min()".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("min(1,8)".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("min([1, 2.0222 ,true,'das', false, null, compiler])".ToEvaluableExpression(option.AsReadOnly())).Evaluate(null));

            var watch = Stopwatch.StartNew();
            var evaluator = compiler.Compile($"sum([{string.Join(",", Enumerable.Range(1, 1000))}])".ToEvaluableExpression());
            Assert.Equal((decimal)Enumerable.Range(1, 1000).Sum(), evaluator.Evaluate(null));
            watch.Stop();
            var elapsedTicks = watch.ElapsedTicks;
            watch.Restart();
            Assert.Equal((decimal)Enumerable.Range(1, 1000).Sum(), evaluator.Evaluate(null));
            watch.Stop();
            Assert.True(watch.ElapsedTicks < elapsedTicks);
            Assert.Equal(5050m, compiler.Compile($"sum([{string.Join(",", Enumerable.Range(0, 101))}])".ToEvaluableExpression()).Evaluate(null));
            Assert.Equal((decimal)Enumerable.Range(1, 1000).Sum(), compiler.Compile($"sum([{string.Join(",", Enumerable.Range(1, 1000))}])".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("sum()".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("sum(1,8)".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("sum([1, 2.0222 ,true,'das', false, null, compiler])".ToEvaluableExpression(option.AsReadOnly())).Evaluate(null));

            Assert.Equal(50m, compiler.Compile($"avg([{string.Join(",", Enumerable.Range(0, 101))}])".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("avg()".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("avg(1,8)".ToEvaluableExpression()).Evaluate(null));
            Assert.Throws<UnsupportedFunctionCallException>(() => compiler.Compile("avg([1, 2.0222 ,true,'das', false, null, compiler])".ToEvaluableExpression(option.AsReadOnly())).Evaluate(null));
        }

        [Fact(DisplayName = "compile date and time function expression")]
        public void CompileNakeDateAndTimeFunction()
        {
            var compiler = new Core.Compiler();

            void Equal(Func<object> valueGetter, string expr)
            {
                var evaluator = compiler.Compile(expr.ToEvaluableExpression());
                Assert.NotNull(evaluator.Evaluate(null));
                Assert.Equal(valueGetter(), evaluator.Evaluate(null));
            }

            Equal(() => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "now()");
            Equal(() => DateTime.Now.ToString("yyyy/MM"), "now('yyyy/MM')");
            Equal(() => DateTime.Now.ToString("aaabg"), "now('aaabg')");

            Equal(() => DateTime.Today.ToString("yyyy/MM/dd"), "today()");
            Equal(() => DateTime.Today.ToString("yyyy/MM"), "today('yyyy/MM')");
            Equal(() => DateTime.Today.ToString("aaabg"), "today('aaabg')");

            Equal(() => DateTime.Now.ToString("HH:mm:ss"), "time()");
            Equal(() => DateTime.Now.ToString("HH:mm"), "time('HH:mm')");
            Equal(() => DateTime.Now.ToString("aaabg"), "time('aaabg')");

            Equal(() => DateTime.Now.Year, "year()");
            Equal(() => DateTime.Now.Month, "month()");
            Equal(() => DateTime.Now.Day, "day()");
            Equal(() => DateTime.Now.Hour, "hour()");
            Equal(() => DateTime.Now.Minute, "minute()");
            Equal(() => DateTime.Now.Second, "second()");
        }
    }
}
