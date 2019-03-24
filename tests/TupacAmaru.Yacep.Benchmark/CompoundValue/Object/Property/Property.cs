using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.CompoundValue.Object.Property
{
    public class PropertyBenchmark
    {
        private static readonly char[] chars =
            Enumerable.Range('0', 10)
                .Union(Enumerable.Range('a', 26))
                .Union(Enumerable.Range('A', 26))
                .Select(c => (char)c).ToArray();
        private static readonly FixtureForProperty fixture = new FixtureForProperty();
        private static readonly Random random = new Random();
        private static readonly PropertyInfo propertyInfo;
        private static readonly IEvaluator evaluator;
        private static readonly IEvaluator<FixtureForProperty> typeEvaluator;
        private static readonly Func<FixtureForProperty, string> reader;
        private static readonly string value;
        static PropertyBenchmark()
        {
            var propertyName = "Netyui";
            evaluator = $"{propertyName}".Compile();
            evaluator.Evaluate(fixture);
            typeEvaluator = $"{propertyName}".Compile<FixtureForProperty>();
            typeEvaluator.Evaluate(fixture);
            var obj = Expression.Parameter(typeof(FixtureForProperty), "fixture");
            propertyInfo = typeof(FixtureForProperty).GetProperty(propertyName);
            reader = Expression.Lambda<Func<FixtureForProperty, string>>(Expression.Property(obj, propertyInfo), "ReadObjectPropertyUseDelegate", new[] { obj }).Compile();
            value = new string(Enumerable.Range(0, 100).Select(x => chars[random.Next(0, chars.Length)]).ToArray());
            propertyInfo.SetValue(fixture, value);
        }

        [Benchmark]
        public void DirectRead()
        {
            var result = fixture.Netyui;
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
            var result = r.Netyui;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseReflection()
        {
            var result = propertyInfo.GetValue(fixture) as string;
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

        [Benchmark]
        public void UseTypedCompile()
        {
            var result = typeEvaluator.Evaluate(fixture) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }
    }
}
