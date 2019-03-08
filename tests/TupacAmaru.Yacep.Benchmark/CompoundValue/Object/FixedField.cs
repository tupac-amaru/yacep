using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.CompoundValue.Object
{
    public class FixedFieldBenchmark
    {
        private static readonly char[] chars =
            Enumerable.Range('0', 10)
                .Union(Enumerable.Range('a', 26))
                .Union(Enumerable.Range('A', 26))
                .Select(c => (char)c).ToArray();
        private readonly FixtureForField fixture = new FixtureForField();
        private static readonly Random random = new Random();
        private static readonly string fieldName = "lxxrt";
        private static readonly FieldInfo fieldInfo = typeof(FixtureForField).GetField(fieldName);
        private static readonly IEvaluator evaluator = $"{fieldName}".Compile();
        private static readonly Func<FixtureForField, string> reader;

        static FixedFieldBenchmark()
        {
            var obj = Expression.Parameter(typeof(FixtureForField), "fixture");
            reader = Expression.Lambda<Func<FixtureForField, string>>(Expression.Field(obj, fieldInfo), "ReadObjectFieldUseDelegate", new[] { obj }).Compile();
        }

        private string value;

        [Params(false, true)]
        public bool WarmUp;

        [GlobalSetup]
        public void Setup()
        {
            value = new string(Enumerable.Range(0, 100).Select(x => chars[random.Next(0, chars.Length)]).ToArray());
            fieldInfo.SetValue(fixture, value);
            if (WarmUp)
            {
                UseDynamic();
                UseYacep();
                UseDelegate();
                UseReflection();
            }
        }

        [Benchmark]
        public void UseDelegate()
        {
            var result = reader(fixture);
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseDynamic()
        {
            dynamic r = fixture;
            var result = r.lxxrt;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseReflection()
        {
            var result = fieldInfo.GetValue(fixture) as string;
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
