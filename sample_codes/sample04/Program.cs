using System;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Extensions;

namespace Sample04
{
    class Program
    {
        static void Main()
        {
            var option = new ParseOption()
                .NotAllowedArrayExpression() //不允许分析包含数组表达式的字符串
                .NotAllowedInExpression() //不允许分析包含In表达式的字符串
                .AddLiteralValue("one_hundred", 100) //添加字面值oneHundred，代表数字100
                .AddLiteralValue("two_thousand", 2000) //添加字面值twoThousand，代表数字2000
                .AddLiteralValue("two_thousand_and_one", 2100) //添加字面值twoThousand，代表数字2000
                .AddBinaryOperator("plus", (a, b) => (int)a + (int)b, 2) //添加二元操作符plus，优先级为2
                .AddBinaryOperator("is", (a, b) => (int)a == (int)b, 1) //添加二元操作符is，优先级为1，数字越小优先级越低
                .AsReadOnly();
            Console.WriteLine("100 plus 2000 is 2100".Compile(option).EvaluateAs<bool>());//true
            Console.WriteLine("one_hundred plus two_thousand is two_thousand_and_one".Compile(option).EvaluateAs<bool>());//true
            Console.WriteLine("one_hundred plus two_thousand is 2100".Compile(option).EvaluateAs<bool>());//true
        }
    }
}
