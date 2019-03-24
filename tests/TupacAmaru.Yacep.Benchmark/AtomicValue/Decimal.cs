using BenchmarkDotNet.Attributes;
using System;
using System.Globalization;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.AtomicValue
{
    public class DecimalBenchmark
    {
        private static readonly Random random = new Random();
        private IEvaluator evaluator;
        private decimal value;

        [Params(5, 10, 15, 50)]
        public int ValueLength;
        [Params(false, true)]
        public bool WarmUp;

        [GlobalSetup]
        public void Setup()
        {
            var max = (int)Math.Pow(10, ValueLength);
            value = (decimal)random.NextDouble() * max;
            var expr = value.ToString(CultureInfo.InvariantCulture);
            evaluator = expr.Compile();
            if (WarmUp)
            {
                evaluator.EvaluateAs<decimal>();
            }
        }

        [Benchmark]
        public void EvaluateDecimal()
        {
            var result = evaluator.EvaluateAs<decimal>();
            if (result != value)
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }
    }
}
