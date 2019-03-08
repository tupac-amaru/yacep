using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.AtomicValue
{
    public class IdentifierBenchmark
    { 
        private IEvaluator evaluator;
        private ReadOnlyDictionary<string, int> state;
        private decimal total;

        [Params(5, 10, 20, 30)]
        public int IdentifierCount;
        [Params(false, true)]
        public bool WarmUp;

        [GlobalSetup]
        public void Setup()
        {
            var dictionary = new Dictionary<string, int>();
            var expr = "0";
            for (var i = 0; i < IdentifierCount; i++)
            {
                var name = $"v{i}";
                var value = i;
                total += value;
                expr = $"{expr}+{name}";
                dictionary[name] = value;
            }
            evaluator = expr.Compile();
            state = new ReadOnlyDictionary<string, int>(dictionary);
            if (WarmUp)
            {
                evaluator.EvaluateAs<decimal>(state);
            }
        }

        [Benchmark]
        public void EvaluateIdentifier()
        {
            var result = evaluator.EvaluateAs<decimal>(state);
            if (result != total)
                throw new Exception($"evaluate failed,result:{result},total:{total}");
        }
    }
}
