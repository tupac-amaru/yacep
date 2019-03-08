using BenchmarkDotNet.Attributes;
using System;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.AtomicValue
{
    public class NakedFunctionBenchmark
    {
        private static readonly Random random = new Random();
        private IEvaluator evaluator;
        private decimal total;

        [Params(1, 5, 10, 20, 30)]
        public int FunctionCount;
        [Params(false, true)]
        public bool WarmUp;
        [Params(false, true)]
        public bool Cachable;

        [GlobalSetup]
        public void Setup()
        {
            var option = ParseOption.CreateOption();
            var expr = "0";
            for (var i = 0; i < FunctionCount; i++)
            {
                var name = $"f{i}";
                var value = i;
                option.AddNakedFunction(name, s => value, Cachable);
                total += value;
                expr = $"{expr}+{name}()";
            }
            evaluator = expr.Compile(option.AsReadOnly());
            if (WarmUp)
            {
                evaluator.EvaluateAs<decimal>();
            }
        }

        [Benchmark]
        public void EvaluateNakedFunction()
        {
            var result = evaluator.EvaluateAs<decimal>();
            if (result != total) throw new Exception($"evaluate failed,result:{result},total:{total}");
        } 
    }
}
