using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.CompoundValue
{
    public class CompileGenericObjectExpressionUnitTest
    {
        [Fact(DisplayName = "determined type compile trick expression")]
        public void CompileTrick()
        {
            var compiler = Core.Compiler.Default;
            Assert.Equal(4, "('ab'+'cd').Length".ToEvaluableExpression().Compile<Fixture>().Evaluate(null));
            Assert.Equal(4, "('ab'+'cd').Length".ToEvaluableExpression().Compile<Fixture>(compiler).Evaluate(null));

            Assert.Equal(4, "this[v?'y':'x'][len]".Compile<Fixture>(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", true)
                    .AddLiteralValue("len", "Length")
                    .AsReadOnly()
            ).Evaluate(new Fixture { y = "abcd" }));

            Assert.Equal(4, "this[v]['Len'+'gth']".Compile<Fixture>(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", "y")
                    .AsReadOnly()
            ).Evaluate(new Fixture { y = "abcd" }));

            Assert.Equal(4, "this['y'][v]".Compile<Fixture>(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", "Length")
                    .AsReadOnly()
            ).Evaluate(new Fixture { y = "abcd" }));

            Assert.Equal(4, "this[v]['Length']".Compile<Fixture>(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", "y")
                    .AsReadOnly()
            ).Evaluate(new Fixture { y = "abcd" }));

            Assert.Equal(4, "('ab'+'cd').Length".Compile<Fixture>().Evaluate(null));

            Assert.Equal("aa", "this[v?'D':'D']()".Compile<Fixture>(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", true)
                    .AsReadOnly()
                ).EvaluateAs<Fixture, string>(new Fixture { D = new Func<string>(() => "aa") }));

            Assert.Throws<UnsupportedObjectIndexerException>(() => "this[v]".Compile<Fixture>(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", compiler)
                    .AsReadOnly()
            ).Evaluate(new Fixture()));
        }

        [Fact(DisplayName = "determined type compile dictionary object")]
        public void CompileDictionary()
        {
            var compiler = Core.Compiler.Default;

            Assert.Equal(222, "this[2]".Compile<Dictionary<int, int>>().Evaluate(new Dictionary<int, int>
            {
                [1] = 111,
                [2] = 222
            }));
            Assert.Equal("b", "this[2]".Compile<Dictionary<int, object>>().Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = "b"
            }));
            Assert.Equal("b", "this[1+1]".Compile<Dictionary<int, object>>().Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = "b"
            }));
            Assert.Equal("b", "this[1+v]".Compile<Dictionary<int, object>>(
                ParseOption.CreateOption()
                    .AddLiteralValue("v", 1)
                    .AsReadOnly()
                ).Evaluate(new Dictionary<int, object>
                {
                    [1] = "a",
                    [2] = "b"
                }));
            Assert.Equal(2147483649L, "this[2]".Compile<Dictionary<int, object>>().Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = 2147483649L
            }));

            Assert.Equal(compiler, "this['a']".Compile<Dictionary<string, object>>().Evaluate(new Dictionary<string, object>
            {
                ["a"] = compiler,
                ["b"] = "b"
            }));
            Assert.Equal(compiler, "this.a".Compile<dynamic>().Evaluate(new
            {
                a = compiler,
                b = "b"
            }));
            Assert.Equal(compiler, "this['a']".Compile<dynamic>().Evaluate(new
            {
                a = compiler,
                b = "b"
            }));
            Assert.Equal(3428392, "this.b".Compile<Dictionary<string, int>>().Evaluate(new Dictionary<string, int>
            {
                ["a"] = 456237,
                ["b"] = 3428392
            }));
            Assert.Equal(3428392, "b".Compile<Dictionary<string, int>>().Evaluate(new Dictionary<string, int>
            {
                ["a"] = 456237,
                ["b"] = 3428392
            }));
            Assert.Equal(3428392, "b".Compile<Hashtable>().Evaluate(new Hashtable
            {
                ["a"] = 456237,
                ["b"] = 3428392
            }));
            Assert.Equal(compiler, "a".Compile<Dictionary<string, object>>().Evaluate(new Dictionary<string, object>
            {
                ["a"] = compiler,
                ["b"] = "b"
            }));
            Assert.Equal("aaa", "this.a()".Compile<Dictionary<string, object>>().Evaluate(new Dictionary<string, object>
            {
                ["a"] = new Func<object>(() => "aaa"),
                ["b"] = "b"
            }));
            Assert.True("this.a(null)".Compile<Dictionary<string, object>>().EvaluateAs<Dictionary<string, object>, bool>(new Dictionary<string, object>
            {
                ["a"] = new Func<object, object>(s => s == null),
                ["b"] = "b"
            }));
            Assert.Null("this.a(null)".Compile<Dictionary<string, Func<object, object>>>().EvaluateAs<Dictionary<string, Func<object, object>>, object>(new Dictionary<string, Func<object, object>> { ["a"] = Fixture.ReturnMe }));
            Assert.Null("this.b(null)".Compile<Dictionary<string, Func<object, object>>>().EvaluateAs<Dictionary<string, Func<object, object>>, object>(new Dictionary<string, Func<object, object>> { ["b"] = Fixture.ReturnMe }));
            Assert.Null("this.a(null)".Compile<Dictionary<string, Action>>().EvaluateAs<Dictionary<string, Action>, object>(new Dictionary<string, Action> { ["a"] = Fixture.DoEmpty }));

            Assert.Throws<UnsupportedFunctionException>(() => compiler.Compile<Dictionary<string, object>>("this.a('111')".ToEvaluableExpression())
                .Evaluate(new Dictionary<string, object>
                {
                    ["a"] = "11".Compile().EvaluateAs<int>() == 11 ? (Func<string>)null : () => "emptyFunc"
                }));
        }

        [Fact(DisplayName = "determined type compile object member")]
        public void CompileObjectMemberExpression()
        {
            Assert.Throws<ArgumentNullException>(() => Core.Compiler.Default.Compile<Fixture>(null));
            var cachedEvaluator = "3*'abc'".Compile<Fixture>();
            Assert.Equal("abcabcabc", cachedEvaluator.EvaluateAs<Fixture, string>(null));
            Assert.Equal("abcabcabc", cachedEvaluator.EvaluateAs<Fixture, string>(null));
            var fixture = new Fixture()
            {
                x = 12,
                y = "aaa",
                a = "x",
                c = () => "string"
            };
            Assert.Equal(3, "abc".Compile<IndexerObject>().EvaluateAs<IndexerObject, int>(new IndexerObject()));
            Assert.Equal(fixture.a, "this.a".Compile<Fixture>().EvaluateAs<Fixture, string>(fixture));
            Assert.Equal(fixture.a, "this[thisIsA]".Compile<Fixture>(
                ParseOption.CreateOption()
                .AddLiteralValue("thisIsA", "a")
                .AsReadOnly()
            ).EvaluateAs<Fixture, string>(fixture));
            Assert.Equal("aa", "this.a()".Compile<Fixture>().EvaluateAs<Fixture, string>(new Fixture { a = new Func<string>(() => "aa") }));
            Assert.Equal("aa", "this.D()".Compile<Fixture>().EvaluateAs<Fixture, string>(new Fixture { D = new Func<string>(() => "aa") }));
            Assert.Equal("12122", "this.D".Compile<Fixture>().EvaluateAs<Fixture, string>(new Fixture { D = "12122" }));
            Assert.Equal("12122", "D".Compile<Fixture>().EvaluateAs<Fixture, string>(new Fixture { D = "12122" }));
            Assert.Equal(12122, "E".Compile<Fixture>().EvaluateAs<Fixture, int>(new Fixture { E = 12122 }));
            Assert.Equal("this is a value", "a".Compile<Fixture>().EvaluateAs<Fixture, string>(new Fixture { a = "this is a value" }));
            Assert.Equal(123456, "x".Compile<Fixture>().EvaluateAs<Fixture, int>(new Fixture { x = 123456 }));
            Assert.Equal(fixture.ToString(), "this.ToString()".Compile<Fixture>().EvaluateAs<Fixture, string>(fixture));
            Assert.Equal(fixture.c(), "this.c()".Compile<Fixture>().EvaluateAs<Fixture, string>(fixture));
            Assert.Equal(fixture.c(), "this.c()".Compile<Fixture>(Core.Parser.Default).EvaluateAs<Fixture, string>(fixture));
            Assert.Equal(fixture.ToString(), "this.ToString()".Compile<Fixture>().EvaluateAsType(fixture, typeof(string)));
            Assert.Equal(fixture.GetString("abc"), "this.GetString('abc')".Compile<Fixture>().Evaluate(fixture));
            Assert.Equal(fixture.Add(12), "this.Add(12)".Compile<Fixture>().Evaluate(fixture));
            Assert.Equal(fixture.Add(12).ToString(), "this.Add(12)".Compile<Fixture>().EvaluateAs<Fixture, string>(fixture));
            Assert.Null("this.DoSomething()".Compile<Fixture>().Evaluate(fixture));
            Assert.Throws<MemberNotFoundException>(() => "Y".Compile<Fixture>().Evaluate(fixture));
            Assert.Throws<MemberNotFoundException>(() => "this.Y".Compile<Fixture>().Evaluate(fixture));
        }

        [Fact(DisplayName = "determined type compile multi level object member")]
        public void CompileMultiLevelObjectMemberExpression()
        {
            var a = new A
            {
                b = new A.B
                {
                    c = new A.B.C
                    {
                        d = new A.B.C.D
                        {
                            e = "this is e"
                        }
                    }
                }
            };
            Assert.Equal("this is e", "b['c']['d']['e']".Compile<A>().Evaluate(a));
            Assert.Equal("this is e", "b['c'][thisIsD]['e']".Compile<A>(
                ParseOption.CreateOption()
                    .AddLiteralValue("thisIsD", "d")
                    .AsReadOnly()
            ).Evaluate(a));
            Assert.Equal("this is e", "b.c.d.e".Compile<A>().Evaluate(a));
            Assert.Equal("this is e", "this.b.c.d.e".Compile<A>().Evaluate(a));
            Assert.Equal(a.b, "b".Compile<A>().Evaluate(a));
        }

        [Fact(DisplayName = "determined type compile dynamic object")]
        public void CompileExpandoObject()
        {
            var compiler = Core.Compiler.Default;
            dynamic expandoObject = new ExpandoObject();
            expandoObject.a = compiler;
            expandoObject.b = "b";
            Assert.Equal(compiler, "this['a']".Compile<ExpandoObject>().Evaluate(expandoObject));
            Assert.Equal(compiler, "a".Compile<ExpandoObject>().Evaluate(expandoObject));
            expandoObject.a = new Func<object>(() => "aaa");
            expandoObject.b = "b";
            Assert.Equal("aaa", "this.a()".Compile<ExpandoObject>().Evaluate(expandoObject));
            expandoObject.a = new Func<object, object>(s => s == null);
            expandoObject.b = "b";
            Assert.True("this.a(null)".Compile<ExpandoObject>().Evaluate(expandoObject));
        }
    }
}
