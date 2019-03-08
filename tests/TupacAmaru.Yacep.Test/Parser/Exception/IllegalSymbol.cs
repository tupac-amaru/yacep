using System;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Symbols;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.Exception
{
    public class ParseIllegalSymbolUnitTest
    {
        [Fact(DisplayName = "throw exception when binary operator contains spaces")]
        public void UnsupportedBinaryOperatorContainsSpaces()
        {
            Assert.Throws<CannotContainsSpacesException>(() => new BinaryOperator("de mo", null, 90));
            Assert.Throws<CannotContainsSpacesException>(() => new BinaryOperator("de　mo", null, 90));
            Assert.Throws<CannotContainsSpacesException>(() => new BinaryOperator("de\tmo", null, 90));
            Assert.Throws<CannotContainsSpacesException>(() => new BinaryOperator("de\rmo", null, 90));
            Assert.Throws<CannotContainsSpacesException>(() => new BinaryOperator("de\nmo", null, 90));
            Assert.Throws<CannotContainsSpacesException>(() => new BinaryOperator("de\bmo", null, 90));
        }
        [Fact(DisplayName = "throw exception when literal value is delegate")]
        public void UnsupportedLiteralValues()
        {
            Assert.Throws<UnsupportedLiteralValueException>(() => new LiteralValue("demo", new Func<int>(() => 1)));
            Assert.Throws<UnsupportedLiteralValueException>(() => new LiteralValue("demo", new Func<int, int>(x => x)));
        }
        [Fact(DisplayName = "throw exception when literal contains spaces")]
        public void UnsupportedLiteralContainsSpaces()
        {
            Assert.Throws<CannotContainsSpacesException>(() => new LiteralValue("de mo", "11"));
            Assert.Throws<CannotContainsSpacesException>(() => new LiteralValue("de　mo", "11"));
            Assert.Throws<CannotContainsSpacesException>(() => new LiteralValue("de\tmo", "11"));
            Assert.Throws<CannotContainsSpacesException>(() => new LiteralValue("de\rmo", "11"));
            Assert.Throws<CannotContainsSpacesException>(() => new LiteralValue("de\nmo", "11"));
            Assert.Throws<CannotContainsSpacesException>(() => new LiteralValue("de\bmo", "11"));
        }
        [Fact(DisplayName = "throw exception when naked function name contains spaces")]
        public void UnsupportedNakedFunctionNameContainsSpaces()
        {
            Assert.Throws<CannotContainsSpacesException>(() => new NakedFunction("de mo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new NakedFunction("de　mo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new NakedFunction("de\tmo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new NakedFunction("de\rmo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new NakedFunction("de\nmo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new NakedFunction("de\bmo", null));
        }
        [Fact(DisplayName = "throw exception when unary operator contains spaces")]
        public void UnsupportedUnaryOperatorContainsSpaces()
        {
            Assert.Throws<CannotContainsSpacesException>(() => new UnaryOperator("de mo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new UnaryOperator("de　mo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new UnaryOperator("de\tmo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new UnaryOperator("de\rmo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new UnaryOperator("de\nmo", null));
            Assert.Throws<CannotContainsSpacesException>(() => new UnaryOperator("de\bmo", null));
        }
    }
}
