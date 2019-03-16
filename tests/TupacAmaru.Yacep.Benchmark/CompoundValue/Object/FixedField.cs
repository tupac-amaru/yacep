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
        private static readonly FixtureForField fixture = new FixtureForField();
        private static readonly Random random = new Random();
        private static readonly FieldInfo fieldInfo;
        private static readonly IEvaluator evaluator;
        private static readonly Func<FixtureForField, string> reader;
        private static readonly string value;
        static FixedFieldBenchmark()
        {
            var obj = Expression.Parameter(typeof(FixtureForField), "fixture");
            var fieldName = "xchjjtool";
            fieldInfo = typeof(FixtureForField).GetField(fieldName);
            reader = Expression.Lambda<Func<FixtureForField, string>>(Expression.Field(obj, fieldInfo), "ReadObjectFieldUseDelegate", new[] { obj }).Compile();
            evaluator = $"{fieldName}".Compile();
            evaluator.Evaluate(fixture);

            value = new string(Enumerable.Range(0, 100).Select(x => chars[random.Next(0, chars.Length)]).ToArray());
            fieldInfo.SetValue(fixture, value);
        }

        [Benchmark]
        public void DirectRead()
        {
            var result = fixture.xchjjtool;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
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
            var result = r.xchjjtool;
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
