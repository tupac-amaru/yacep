using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.CompoundValue.Object
{
    public class RandomFieldBenchmark
    {
        private static readonly char[] chars =
            Enumerable.Range('0', 10)
                .Union(Enumerable.Range('a', 26))
                .Union(Enumerable.Range('A', 26))
                .Select(c => (char)c).ToArray();

        private readonly FixtureForField fixture = new FixtureForField();
        private static readonly Random random = new Random();
        private string value;
        private string fieldName;
        private IEvaluator evaluator;

        [Params(false, true)]
        public bool WarmUp;

        [GlobalSetup]
        public void Setup()
        {
            var fieldNames = typeof(FixtureForField).GetFields().Select(x => x.Name).ToArray();
            fieldName = fieldNames[random.Next(0, fieldNames.Length)];
            var fieldInfo = typeof(FixtureForField).GetField(fieldName);
            value = new string(Enumerable.Range(0, 100).Select(x => chars[random.Next(0, chars.Length)]).ToArray());
            fieldInfo.SetValue(fixture, value);
            evaluator = $"{fieldName}".Compile();
            if (WarmUp)
            {
                UseYacep();
                UseReflection();
            }
        }

        [Benchmark]
        public void UseReflection()
        {
            var result = typeof(FixtureForField).GetField(fieldName).GetValue(fixture) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseYacep()
        {
            var result = evaluator.Evaluate(fixture) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }
    }
}
