using TupacAmaru.Yacep.Extensions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Compiler.AtomicValue
{
    public class CompileStringUnitTest
    {
        [Fact(DisplayName = "compile integer expression")]
        public void CompileString()
        {
            Assert.Equal("abasdkas", "'abasdkas'".Compile().EvaluateAs<string>());
            Assert.Equal("121asadas\t1uhj21", "'121asadas\t1uhj21'".Compile().EvaluateAs<string>());
            Assert.Equal("twew\tqqwkqekkk", "'twew\\tqqwkqekkk'".Compile().EvaluateAs<string>());
            Assert.Equal("twew\tqqwkqekkk", "'twew\\tqqwkqekkk'".Compile().EvaluateAsType(typeof(string)));
        }
    }
}
