using System;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Extensions;

namespace Sample07
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("'b'@'abc'".Compile(new ParseOption()
                                   .AddBinaryOperator("@", (a, b) => (b as string)?.IndexOf(a as string) ?? 0, 10) //获取子串位置
                                   .AsReadOnly())
                               .EvaluateAs<double>()); //1
            Console.WriteLine("'b'I'abc'".Compile(new ParseOption()
                                   .AddBinaryOperator("I", (a, b) => (b as string)?.IndexOf(a as string) ?? 0, 10) //获取子串位置
                                   .AsReadOnly())
                               .EvaluateAs<double>()); //1
            Console.WriteLine("'hello' concat 'world'".Compile(new ParseOption()
                       .AddBinaryOperator("concat", (a, b) => $"{a}, {b}", 10) //获取子串位置
                       .AsReadOnly())
                   .EvaluateAs<string>()); //hello, world
        }
    }
}
