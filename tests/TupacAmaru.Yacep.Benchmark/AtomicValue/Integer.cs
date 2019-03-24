using BenchmarkDotNet.Attributes;
using System;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.AtomicValue
{
    public class IntegerBenchmark
    { 
        private static readonly Random random = new Random();
        private IEvaluator evaluator;
        private int value;

        [Params(1, 2, 3, 4, 5, 6, 7, 8)]
        public int ValueLength;
        [Params(false, true)]
        public bool WarmUp;

        [GlobalSetup]
        public void Setup()
        {
            var max = (int)Math.Pow(10, ValueLength);
            value = random.Next(0, max);
            var expr = value.ToString();
            evaluator = expr.Compile();
            if (WarmUp)
            {
                evaluator.EvaluateAs<decimal>();
            }
        }

        [Benchmark]
        public void EvaluateInteger()
        {
            var result = evaluator.EvaluateAs<int>();
            if (result != value) throw new Exception($"evaluate failed,result:{result},value:{value}");
        }
    }
}
