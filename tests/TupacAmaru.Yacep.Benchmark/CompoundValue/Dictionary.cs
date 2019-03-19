using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.CompoundValue
{
    public class DictionaryBenchmark
    {
        private static readonly char[] chars =
            Enumerable.Range('0', 10)
                .Union(Enumerable.Range('a', 26))
                .Union(Enumerable.Range('A', 26))
                .Select(c => (char)c).ToArray();
        private static readonly Random random = new Random();
        private static readonly Func<Dictionary<string, string>, string, string> reader;
        static DictionaryBenchmark()
        {
            var dict = Expression.Parameter(typeof(Dictionary<string, string>), "dict");
            var key = Expression.Parameter(typeof(string), "key");
            methodInfo = typeof(Dictionary<string, string>).GetMethod("get_Item");
            reader = Expression.Lambda<Func<Dictionary<string, string>, string, string>>(Expression.Call(dict, methodInfo, key),
                "GetDictionaryValue", new[] { dict, key }).Compile();
        }
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();
        private static readonly MethodInfo methodInfo;
        private string value;
        private string key;
        private IEvaluator evaluator;
        [Params(10, 50, 100, 1000, 10000)]
        public int ItemCount;

        [GlobalSetup]
        public void Setup()
        {
            for (var i = 0; i < ItemCount; i++)
            {
                dictionary[$"k{i}"] = new string(Enumerable.Range(0, 100).Select(x => chars[random.Next(0, chars.Length)]).ToArray());
            }
            key = $"k{random.Next(0, ItemCount)}";
            value = dictionary[key];
            evaluator = $"this['{key}']".Compile();
            evaluator.Evaluate(dictionary);
        }


        [Benchmark]
        public void DirectRead()
        {
            var result = dictionary[key];
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseDelegate()
        {
            var result = reader(dictionary, key);
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseDynamic()
        {
            dynamic r = dictionary;
            var result = r[key];
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseReflection()
        {
            var result = methodInfo.Invoke(dictionary, new object[] { key }) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }

        [Benchmark]
        public void UseYacep()
        {
            var result = evaluator.Evaluate(dictionary) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
        }
    }
}
