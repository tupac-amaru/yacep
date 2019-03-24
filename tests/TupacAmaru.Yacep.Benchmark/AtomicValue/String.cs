using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.AtomicValue
{
    public class StringBenchmark
    {
        private static readonly char[] chars =
            Enumerable.Range('0', 10)
                .Union(Enumerable.Range('a', 26))
                .Union(Enumerable.Range('A', 26))
                .Select(c => (char)c).ToArray();
        private static readonly Random random = new Random();
        private IEvaluator evaluator;
        private string value;

        [Params(10, 20, 30, 40, 100, 1000)]
        public int StringLength;

        [GlobalSetup]
        public void Setup()
        {
            value = new string(Enumerable.Range(0, StringLength).Select(x => chars[random.Next(0, chars.Length)]).ToArray());
            evaluator = $"'{value}'".Compile();
        }

        [Benchmark]
        public void EvaluateString()
        {
            var result = evaluator.EvaluateAs<string>();
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }
    }
}
