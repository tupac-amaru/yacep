using BenchmarkDotNet.Attributes;
using System;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.AtomicValue
{
    public class LiteralBenchmark
    {
        private decimal total;
        private IEvaluator evaluator;
        [Params(5, 10, 20, 30)]
        public int LiteralCount;
        [Params(false, true)]
        public bool WarmUp; 

        [GlobalSetup]
        public void Setup()
        {
            var option = ParseOption.CreateOption();
            var expr = "0";
            for (var i = 0; i < LiteralCount; i++)
            {
                var name = $"v{i}";
                var value = i;
                total += value;
                expr = $"{expr}+{name}";
                option.AddLiteralValue(name, value );
            }
            evaluator = expr.Compile(option.AsReadOnly());
            if (WarmUp)
            {
                evaluator.EvaluateAs<decimal>();
            }
        }
        [Benchmark]
        public void EvaluateLiteral()
        {
            var result = evaluator.EvaluateAs<decimal>();
            if (result != total)
                throw new Exception($"evaluate failed,result:{result},total:{total}");
        }
    }
}
