using System;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.Exception
{
    public class ParseErrorExpressionUnitTest
    {
        [Fact(DisplayName = "throw exception for illegal expression")]
        public void ErrorExpression()
        {
            var parser = new Core.Parser();
            Assert.Throws<ArgumentException>(() => parser.Parse(""));
            Assert.Throws<ArgumentException>(() => parser.Parse(null));
            Assert.Throws<ArgumentException>(() => parser.Parse("    "));

            Assert.Equal("Unexpected \"#\" at character 0", Assert.Throws<ParseException>(() => parser.Parse("#aaa")).Message);
            Assert.Equal("Unexpected \"@\" at character 0", Assert.Throws<ParseException>(() => parser.Parse("@aaa")).Message);
            Assert.Equal("Unexpected \"@\" at character 4", Assert.Throws<ParseException>(() => parser.Parse("$aa @a)")).Message);
            Assert.Equal("Unexpected \0 at character 2", Assert.Throws<ParseException>(() => parser.Parse("x.")).Message);
            Assert.Equal("Unclosed ( at character 1", Assert.Throws<ParseException>(() => parser.Parse("(")).Message);
            Assert.Equal("Expected ] at character 1", Assert.Throws<ParseException>(() => parser.Parse("[")).Message);
            Assert.Equal("Expected ) at character 5", Assert.Throws<ParseException>(() => parser.Parse("x.f(a")).Message);
            Assert.Equal("Unclosed ( at character 1", Assert.Throws<ParseException>(() => parser.Parse("(@11)")).Message);
            Assert.Equal("Unclosed ( at character 1", Assert.Throws<ParseException>(() => parser.Parse("(#aa)")).Message);
            Assert.Equal("Unclosed ( at character 5", Assert.Throws<ParseException>(() => parser.Parse("($aa @a)")).Message);
            Assert.Equal("Unclosed [ at character 2", Assert.Throws<ParseException>(() => parser.Parse("x[")).Message);
            Assert.Equal("Unclosed quote after \"\" at character 3", Assert.Throws<ParseException>(() => parser.Parse("x['")).Message);
            Assert.Equal("Expected ] at character 6", Assert.Throws<ParseException>(() => parser.Parse("x.f([1")).Message);
            Assert.Equal("Expected , at character 5", Assert.Throws<ParseException>(() => parser.Parse("x.f([)")).Message);
            Assert.Equal("Unexpected token , at character 6", Assert.Throws<ParseException>(() => parser.Parse("max([,")).Message);
            Assert.Equal("Unexpected token , at character 2", Assert.Throws<ParseException>(() => parser.Parse("[,,,]")).Message);
            Assert.Equal("Unexpected token , at character 4", Assert.Throws<ParseException>(() => parser.Parse("[a,,]")).Message);
            Assert.Equal("Expected , at character 6", Assert.Throws<ParseException>(() => parser.Parse("avg((),aa)")).Message);
            Assert.Equal("Expected , at character 13", Assert.Throws<ParseException>(() => parser.Parse("avg([1,2,t,],#aa)")).Message);
            Assert.Equal("Unexpected \"a\" at character 5", Assert.Throws<ParseException>(() => parser.Parse("a in aa")).Message);

            Assert.Equal("Expected expression after % at character 2", Assert.Throws<ParseException>(() => parser.Parse("2%")).Message);
            Assert.Equal("Variable names cannot start with a number (10a) at character 2", Assert.Throws<ParseException>(() => parser.Parse("10a")).Message);
            Assert.Equal("Unexpected period at character 3", Assert.Throws<ParseException>(() => parser.Parse("10..")).Message);
            Assert.Equal("Expected exponent (10e) at character 3", Assert.Throws<ParseException>(() => parser.Parse("10er")).Message);

            Assert.Equal("Unclosed quote after \"abasdkas\" at character 9", Assert.Throws<ParseException>(() => parser.Parse("'abasdkas")).Message);
            Assert.Equal("Unclosed quote after \"rualkl\" at character 7", Assert.Throws<ParseException>(() => parser.Parse("\"rualkl")).Message);
            Assert.Equal("Unclosed quote after \"bm121\\u22a\" at character 11", Assert.Throws<ParseException>(() => parser.Parse("\"bm121\\u22a")).Message);
            Assert.Equal("Unclosed quote after \"bm121\" at character 7", Assert.Throws<ParseException>(() => parser.Parse("\"bm121\\")).Message);

            Assert.Equal("Expected expression at character 2", Assert.Throws<ParseException>(() => parser.Parse("x?")).Message);
            Assert.Equal("Expected expression at character 2", Assert.Throws<ParseException>(() => parser.Parse("x?:")).Message);
            Assert.Equal("Expected : at character 3", Assert.Throws<ParseException>(() => parser.Parse("x?p")).Message);
            Assert.Equal("Expected expression at character 4", Assert.Throws<ParseException>(() => parser.Parse("x?p:")).Message);
            Assert.Equal("Expected expression at character 2", Assert.Throws<ParseException>(() => parser.Parse("x?:p")).Message);
            Assert.Equal("Expected expression at character 4", Assert.Throws<ParseException>(() => parser.Parse("x?p:")).Message);

            Assert.Equal("Expected expression after + at character 2", Assert.Throws<ParseException>(() => parser.Parse("x+")).Message);
            Assert.Equal("Expected expression after + at character 4", Assert.Throws<ParseException>(() => parser.Parse("xYY+")).Message);
            Assert.Equal("Expected expression after * at character 4", Assert.Throws<ParseException>(() => parser.Parse("x+y*")).Message);

            Assert.Equal("Unexpected token , at character 5", Assert.Throws<ParseException>(() => parser.Parse("avg(,aa)")).Message);
            Assert.Equal("Unexpected token ) at character 9", Assert.Throws<ParseException>(() => parser.Parse("a[max(c,)]")).Message);

            Assert.Equal("Can not find naked function (f) at character 0", Assert.Throws<ParseException>(() => parser.Parse("f(1,2)")).Message);
            Assert.Equal("Can not find naked function (p) at character 2", Assert.Throws<ParseException>(() => parser.Parse("1+p(1,2)")).Message);

            Assert.Equal("Can not parse value(111111111111111111111111) to ulong at (start:0,end:24)", Assert.Throws<InvalidCastException>(() => parser.Parse("111111111111111111111111")).Message);
            Assert.Equal("Can not parse value(7792281625142643375935439503359228162514264337593543950335.11) to decimal at (start:0,end:61)", Assert.Throws<InvalidCastException>(() => parser.Parse("7792281625142643375935439503359228162514264337593543950335.11")).Message);


            var option = ParseOption.CreateOption()
                .AddBinaryOperator("add", (a, b) => null, 8)
                .AddBinaryOperator("a@", (a, b) => null, 8)
                .AsReadOnly();
            Assert.Throws<ParseException>(() => parser.Parse("12 add ", option));
            Assert.Throws<ParseException>(() => parser.Parse("12 a@", option));
        }
    }
}
