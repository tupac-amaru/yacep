using System;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Extensions;

namespace Sample06
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("π".Compile(new ParseOption()
                                   .AddLiteralValue("π", Math.PI) //数值π
                                   .AsReadOnly())
                               .EvaluateAs<double>()); //3.14159265358979
            Console.WriteLine("T".Compile(new ParseOption()
                                         .AddLiteralValue("T", "T is a very long string") //长字符串
                                        .AsReadOnly())
                                .EvaluateAs<string>()); //T is a very long string
            Console.WriteLine("os_version".Compile(new ParseOption()
                                            .AddLiteralValue("os_version", Environment.OSVersion.Version) //获取字符串长度
                                            .AsReadOnly())
                                .EvaluateAs<Version>()); //10.0.18362.0
        }
    }
}
