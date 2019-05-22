using Microsoft.Extensions.DependencyInjection;
using System;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Core;

namespace Sample02
{
    class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IParser, Parser>()
                .AddSingleton<ICompiler, Compiler>();

            var container = services.BuildServiceProvider();
            var parser = container.GetService<IParser>();
            var compiler = container.GetService<ICompiler>();
            var expression = parser.Parse("x-y+m*12");
            var evaluator = compiler.Compile(expression);
            Console.Write(evaluator.Evaluate(new { x = 110, y = 10, m = 2 }));
        }
    }
}
