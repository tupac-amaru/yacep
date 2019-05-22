using Microsoft.Extensions.DependencyInjection;
using System;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Core;

namespace Sample01
{
    class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IParser, Parser>()
                .AddSingleton<IFormatter, Formatter>();

            var container = services.BuildServiceProvider();
            var parser = container.GetService<IParser>();
            var formatter = container.GetService<IFormatter>();
            var expression = parser.Parse("x+y+m*12");

            Console.Write(formatter.Format(expression));
        }
    }
}
