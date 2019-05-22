using System;
using TupacAmaru.Yacep.Extensions;

namespace Sample03
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("+m".Compile().Evaluate(new { m = 12 }));
            Console.WriteLine("-m".Compile().Evaluate(new { m = 12 }));
            Console.WriteLine("!m".Compile().Evaluate(new { m = 12 }));
        }
    }
}
