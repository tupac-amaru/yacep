using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.CompoundValue
{
    public class CompileObjectExpressionUnitTest
    {
        [Fact(DisplayName = "compile trick expression")]
        public void CompileTrick()
        {
            var compiler = Core.Compiler.Default;
            Assert.Equal(4, "('ab'+'cd').Length".ToEvaluableExpression().Compile().Evaluate(null));
            Assert.Equal(4, "('ab'+'cd').Length".ToEvaluableExpression().Compile(compiler).Evaluate(null));

            Assert.Equal(4, "this[v]['Length']".Compile(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", "y")
                    .AsReadOnly()
            ).Evaluate(new Fixture { y = "abcd" }));
            Assert.Equal(4, "this[v]['Len'+'gth']".Compile(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", "y")
                    .AsReadOnly()
            ).Evaluate(new Fixture { y = "abcd" }));
            Assert.Equal(4, "('ab'+'cd').Length".Compile().Evaluate(null));
            Assert.Throws<UnsupportedObjectIndexerException>(() => "this[v]".Compile(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", compiler)
                    .AsReadOnly()
            ).Evaluate(new Fixture()));
        }

        [Fact(DisplayName = "compile anonymous type object")]
        public void CompileIdentifierFromAnonymousType()
        {
            Assert.Equal(-12, "x".Compile().Evaluate(new { x = -12 }));
            var state = new
            {
                a = new
                {
                    b = new
                    {
                        c = "this is a very long string"
                    }
                }
            };
            Assert.Equal("this is a very long string", "a.b.c".Compile().Evaluate(state));
            Assert.Equal("this is a very long string", "a['b']['c']".Compile().Evaluate(state));
        }

        [Fact(DisplayName = "compile dictionary object")]
        public void CompileDictionary()
        {
            Assert.Equal("b", "this[2]".Compile().Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = "b"
            }));
            Assert.Equal(2147483649L, "this[2]".Compile().Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = 2147483649L
            }));

            var compiler = Core.Compiler.Default;
            Assert.Equal(compiler, "this['a']".Compile().Evaluate(new Dictionary<string, object>
            {
                ["a"] = compiler,
                ["b"] = "b"
            }));
            Assert.Equal(compiler, "a".Compile().Evaluate(new Dictionary<string, object>
            {
                ["a"] = compiler,
                ["b"] = "b"
            }));
            Assert.Equal("aaa", "this.a()".Compile().Evaluate(new Dictionary<string, object>
            {
                ["a"] = new Func<object>(() => "aaa"),
                ["b"] = "b"
            }));
            Assert.True("this.a(null)".Compile().EvaluateAs<bool>(new Dictionary<string, object>
            {
                ["a"] = new Func<object, object>(s => s == null),
                ["b"] = "b"
            }));
            Assert.Null("this.a(null)".Compile().EvaluateAs<object>(new Dictionary<string, Func<object, object>> { ["a"] = Fixture.ReturnMe }));
            Assert.Null("this.b(null)".Compile().EvaluateAs<object>(new Dictionary<string, Func<object, object>> { ["b"] = Fixture.ReturnMe }));
            Assert.Null("this.a(null)".Compile().EvaluateAs<object>(new Dictionary<string, Action> { ["a"] = Fixture.DoEmpty }));

            Assert.Throws<UnsupportedFunctionException>(() => compiler.Compile("this.a('111')".ToEvaluableExpression())
                .Evaluate(new Dictionary<string, object>
                {
                    ["a"] = "11".Compile().EvaluateAs<int>() == 11 ? (Func<string>)null : () => "emptyFunc"
                }));
        }

        [Fact(DisplayName = "compile dynamic object")]
        public void CompileExpandoObject()
        {
            var compiler = Core.Compiler.Default;
            dynamic expandoObject = new ExpandoObject();
            expandoObject.a = compiler;
            expandoObject.b = "b";
            Assert.Equal(compiler, "this['a']".Compile().Evaluate(expandoObject));
            Assert.Equal(compiler, "a".Compile().Evaluate(expandoObject));
            expandoObject.a = new Func<object>(() => "aaa");
            expandoObject.b = "b";
            Assert.Equal("aaa", "this.a()".Compile().Evaluate(expandoObject));
            expandoObject.a = new Func<object, object>(s => s == null);
            expandoObject.b = "b";
            Assert.True("this.a(null)".Compile().Evaluate(expandoObject));
        }

        [Fact(DisplayName = "compile object member")]
        public void CompileObjectMemberExpression()
        {
            var fixture = new Fixture()
            {
                x = 12,
                y = "aaa",
                a = "x",
                c = () => "string"
            };
            Assert.Equal(fixture.a, "this.a".Compile().EvaluateAs<string>(fixture));
            Assert.Equal(fixture.a, "this[thisIsA]".Compile(ParseOption.CreateOption()
                .AddLiteralValue("thisIsA", "a")
                .AsReadOnly()
            ).EvaluateAs<string>(fixture));
            Assert.Equal("aa", "this.a()".Compile().EvaluateAs<string>(new Fixture { a = new Func<string>(() => "aa") }));
            Assert.Equal("aa", "this.D()".Compile().EvaluateAs<string>(new Fixture { D = new Func<string>(() => "aa") }));
            Assert.Equal("12122", "this.D".Compile().EvaluateAs<string>(new Fixture { D = "12122" }));
            Assert.Equal(fixture.ToString(), "this.ToString()".Compile().EvaluateAs<string>(fixture));
            Assert.Equal(fixture.c(), "this.c()".Compile().EvaluateAs<string>(fixture));
            Assert.Equal(fixture.ToString(), "this.ToString()".Compile().EvaluateAsType(fixture, typeof(string)));
            Assert.Throws<ParseException>(() => "ToString()".Compile().Evaluate(fixture));
            Assert.Equal(3, "this.lengthOf('abc')".Compile().Evaluate(new { lengthOf = new Func<string, int>(s => s.Length) }));
            Assert.Equal(fixture.GetString("abc"), "this.GetString('abc')".Compile().Evaluate(fixture));
            Assert.Equal(fixture.Add(12), "this.Add(12)".Compile().Evaluate(fixture));
            Assert.Equal(fixture.Add(12), "a.Add(12)".Compile().Evaluate(new { a = fixture }));
            Assert.Equal("1111", "this.f[0]()".Compile().Evaluate(new { f = new object[] { new Func<string>(() => "1111") } }));
            Assert.Equal(6, "('abc'+d).Length".Compile().Evaluate(new { d = "efg" }));
        }

        [Fact(DisplayName = "compile real object")]
        public void CompileRealObject()
        {
            var state = new
            {
                x = 7,
                y = 43.0f,
                z = new Dictionary<string, string>
                {
                    ["yacep"] = "yet another csharp expression parser",
                    ["tupac-amaru"] = "was the last indigenous monarch (Sapa Inca) of the Neo-Inca State"
                },
                rand = new Func<object>(() => new Random().Next(1, 3)),
                array = Enumerable.Range(1971, 1996 - 1971)
            };
            var expr = "x + y - z['yacep'].Length + max([1, 2, 3]) + (this.rand() > 1 ? 1971 : 1996) - len(array)";
            var evaluator = expr.Compile();
            var value = evaluator.EvaluateAs<decimal>(state);
            Assert.True(value == 1988 || value == 1963);
        }
    }
}
