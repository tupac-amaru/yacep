using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Parser.Exception
{
    public class ParseNotAllowedExpressionUnitTest
    {
        [Fact(DisplayName = "throw exception when configured not allowed array expression")]
        public void NotAllowedArrayExpression()
        {
            var option = ParseOption.CreateOption()
                .NotAllowedArrayExpression()
                .AsReadOnly();
            var parser = new Core.Parser();

            Assert.Equal("Array expression is not allowed at character 0", Assert.Throws<ParseException>(() => parser.Parse("[1,2,3]", option)).Message);
            Assert.Equal("Array expression is not allowed at character 4", Assert.Throws<ParseException>(() => parser.Parse("len([1,2,3])", option)).Message);

            Assert.NotNull(parser.Parse("true?1:2", option) as ConditionalExpression);
            Assert.NotNull(parser.Parse("x.f", option) as ObjectMemberExpression);
            Assert.NotNull(parser.Parse("x['1']", option) as ObjectMemberExpression);
            Assert.NotNull(parser.Parse("x in (true?1:2,m,y)", option) as InExpression);
            var add = parser.Parse("add", option) as IdentifierExpression;
            Assert.NotNull(add);
            Assert.Equal("add", add.Name);
            Assert.NotNull(parser.Parse("12", option) as ConstantExpression);
        }

        [Fact(DisplayName = "throw exception when configured not allowed condition expression")]
        public void NotAllowedConditionExpression()
        {
            var option = ParseOption.CreateOption()
                 .NotAllowedConditionExpression()
                 .AsReadOnly();
            var parser = new Core.Parser();

            Assert.Equal("Condition expression is not allowed at character 0", Assert.Throws<ParseException>(() => parser.Parse("true?1:2", option)).Message);
            Assert.Equal("Condition expression is not allowed at character 6", Assert.Throws<ParseException>(() => parser.Parse("x in (true?1:2,m,y)", option)).Message);

            Assert.NotNull(parser.Parse("[1,2,3]", option) as ArrayExpression);
            Assert.NotNull(parser.Parse("x.f", option) as ObjectMemberExpression);
            Assert.NotNull(parser.Parse("x['1']", option) as ObjectMemberExpression);
            Assert.NotNull(parser.Parse("x in (3,m,y)", option) as InExpression);
            var add = parser.Parse("add", option) as IdentifierExpression;
            Assert.NotNull(add);
            Assert.Equal("add", add.Name);
            Assert.NotNull(parser.Parse("12", option) as ConstantExpression);
        }

        [Fact(DisplayName = "throw exception when configured not allowed member expression")]
        public void NotAllowedMemberExpression()
        {

            var option = ParseOption.CreateOption()
                  .NotAllowedMemberExpression()
                  .AsReadOnly();
            var parser = new Core.Parser();

            Assert.Equal("Member expression is not allowed at character 1", Assert.Throws<ParseException>(() => parser.Parse("x.f", option)).Message);
            Assert.Equal("Member expression is not allowed at character 7", Assert.Throws<ParseException>(() => parser.Parse("x in (m.u,n,y)", option)).Message);

            Assert.NotNull(parser.Parse("[1,2,3]", option) as ArrayExpression);
            Assert.NotNull(parser.Parse("true?1:2", option) as ConditionalExpression);
            Assert.NotNull(parser.Parse("x['1']", option) as ObjectMemberExpression);
            Assert.NotNull(parser.Parse("x in (3,m,y)", option) as InExpression);
            var add = parser.Parse("add", option) as IdentifierExpression;
            Assert.NotNull(add);
            Assert.Equal("add", add.Name);
            Assert.NotNull(parser.Parse("12", option) as ConstantExpression);
        }

        [Fact(DisplayName = "throw exception when configured not allowed indexer expression")]
        public void NotAllowedIndexerExpression()
        {
            var option = ParseOption.CreateOption()
                    .NotAllowedIndexerExpression()
                    .AsReadOnly();
            var parser = new Core.Parser();

            Assert.Equal("Indexer expression is not allowed at character 1", Assert.Throws<ParseException>(() => parser.Parse("x['f']", option)).Message);
            Assert.Equal("Indexer expression is not allowed at character 7", Assert.Throws<ParseException>(() => parser.Parse("x in (m['u'],n,y)", option)).Message);

            Assert.NotNull(parser.Parse("[1,2,3]", option) as ArrayExpression);
            Assert.NotNull(parser.Parse("true?1:2", option) as ConditionalExpression);
            Assert.NotNull(parser.Parse("x.f", option) as ObjectMemberExpression);
            Assert.NotNull(parser.Parse("x in (3,m,y)", option) as InExpression);
            var add = parser.Parse("add", option) as IdentifierExpression;
            Assert.NotNull(add);
            Assert.Equal("add", add.Name);
            Assert.NotNull(parser.Parse("12", option) as ConstantExpression);
        }

        [Fact(DisplayName = "throw exception when configured not allowed in expression")]
        public void NotAllowedInExpression()
        {
            var option = ParseOption.CreateOption()
                .NotAllowedInExpression()
                .AsReadOnly();
            var parser = new Core.Parser();

            Assert.Equal("In expression is not allowed at character 5", Assert.Throws<ParseException>(() => parser.Parse("1 in (1,2,3)", option)).Message);
            Assert.Equal("In expression is not allowed at character 6", Assert.Throws<ParseException>(() => parser.Parse("xy in (m['u'],n,y)", option)).Message);

            Assert.NotNull(parser.Parse("[1,2,3]", option) as ArrayExpression);
            Assert.NotNull(parser.Parse("true?1:2", option) as ConditionalExpression);
            Assert.NotNull(parser.Parse("x.f", option) as ObjectMemberExpression);
            Assert.NotNull(parser.Parse("x['f']", option) as ObjectMemberExpression);
            var add = parser.Parse("add", option) as IdentifierExpression;
            Assert.NotNull(add);
            Assert.Equal("add", add.Name);
            Assert.NotNull(parser.Parse("12", option) as ConstantExpression);
        } 
    }
}
