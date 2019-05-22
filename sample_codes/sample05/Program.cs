using System;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Extensions;

namespace Sample05
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("#'hello'".Compile(new ParseOption()
                                                        .AddUnaryOperator("#", value => (value as string)?.Length ?? 0) //获取字符串长度
                                                        .AsReadOnly())
                                               .EvaluateAs<int>());
            Console.WriteLine("L'hello'".Compile(new ParseOption()
                                                        .AddUnaryOperator("L", value => (value as string)?.Length ?? 0) //获取字符串长度
                                                        .AsReadOnly())
                                               .EvaluateAs<int>());
            Console.WriteLine("LengthOf'hello'".Compile(new ParseOption()
                                                        .AddUnaryOperator("LengthOf", value => (value as string)?.Length ?? 0) //获取字符串长度
                                                        .AsReadOnly())
                                               .EvaluateAs<int>());
        }
    }
}
