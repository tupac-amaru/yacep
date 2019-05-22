using System;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Extensions;

namespace Sample08
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("$('body')".Compile(new ParseOption()
                       .AddNakedFunction("$", values => $"find {values[0]} in dom") //拼接字符串
                       .AsReadOnly())
                   .EvaluateAs<string>()); //find body in dom
            Console.WriteLine("J('body')".Compile(new ParseOption()
                       .AddNakedFunction("J", values => $"find {values[0]} in dom") //拼接字符串
                       .AsReadOnly())
                   .EvaluateAs<string>()); //find body in dom
            Console.WriteLine("jQuery('body')".Compile(new ParseOption()
                       .AddNakedFunction("jQuery", values => $"find {values[0]} in dom") //拼接字符串
                       .AsReadOnly())
                   .EvaluateAs<string>()); //find body in dom
        }
    }
}
