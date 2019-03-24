using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.CompoundValue.Object.Method.WithArguments
{
    public class WithArgumentsMethodBenchmark
    {
        private static readonly char[] chars =
            Enumerable.Range('0', 10)
                .Union(Enumerable.Range('a', 26))
                .Union(Enumerable.Range('A', 26))
                .Select(c => (char)c).ToArray();
        private static readonly FixtureForMethod fixture = new FixtureForMethod();
        private static readonly Random random = new Random();
        private static readonly MethodInfo methodInfo;
        private static readonly IEvaluator evaluator;
        private static readonly IEvaluator<FixtureForMethod> typeEvaluator;
        private static readonly IEvaluator<FixtureForMethod> onlyFunctionEvaluator;
        private static readonly Func<FixtureForMethod, string, string> reader;
        private static readonly string value;
        private static readonly string prefix;
        static WithArgumentsMethodBenchmark()
        {
            var methodName = "Netyui";
            prefix = new string(Enumerable.Range(0, 100).Select(x => chars[random.Next(0, chars.Length)]).ToArray());
            var obj = Expression.Parameter(typeof(FixtureForMethod), "fixture");
            var arg = Expression.Parameter(typeof(string), "arg");
            methodInfo = typeof(FixtureForMethod).GetMethod(methodName);
            reader = Expression.Lambda<Func<FixtureForMethod, string, string>>(Expression.Call(obj, methodInfo, arg), "InvokeObjectMethodUseDelegate", new[] { obj, arg }).Compile();
            value = $"{prefix}{methodName.ToLower()}";
            evaluator = $"this.{methodName}('{prefix}')".Compile();
            evaluator.Evaluate(fixture);
            typeEvaluator = $"this.{methodName}('{prefix}')".Compile<FixtureForMethod>();
            typeEvaluator.Evaluate(fixture);
            onlyFunctionEvaluator = $"this.{methodName}".Compile<FixtureForMethod>();
            onlyFunctionEvaluator.Evaluate(fixture);
        }

        [Benchmark]
        public void DirectRead()
        {
            var result = fixture.Netyui(prefix);
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseDelegate()
        {
            var result = reader(fixture, prefix);
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseDynamic()
        {
            dynamic r = fixture;
            var result = r.Netyui(prefix);
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseReflection()
        {
            var result = methodInfo.Invoke(fixture, new object[] { prefix }) as string;
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

        [Benchmark]
        public void OnlyFunction()
        {
            var result = (onlyFunctionEvaluator.Evaluate(fixture) as Func<object[], object>)?.Invoke(new object[] { prefix }) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }
    }
}
