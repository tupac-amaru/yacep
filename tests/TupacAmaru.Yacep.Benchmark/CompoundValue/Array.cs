using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.CompoundValue
{
    public class ArrayBenchmark
    {
        private static readonly Random random = new Random();
        private IEvaluator evaluateLen;
        private IEvaluator evaluateMax;
        private IEvaluator evaluateMin;
        private IEvaluator evaluateSum;
        private IEvaluator evaluateAvg;
        private int len;
        private int max;
        private int min;
        private int sum;
        private decimal avg;

        [Params(5, 10, 20, 30, 100, 1000)]
        public int NumberCount;

        [GlobalSetup]
        public void Setup()
        {
            len = NumberCount;
            var numbers = Enumerable.Range(0, len).Select(_ => random.Next(-1000, 1000)).ToArray();
            var expr = string.Join(",", numbers);
            max = numbers.Max();
            min = numbers.Min();
            sum = numbers.Sum();
            avg = numbers.Select(x => (decimal)x).Average();
            evaluateLen = $"len([{expr}])".Compile();
            evaluateMax = $"max([{expr}])".Compile();
            evaluateMin = $"min([{expr}])".Compile();
            evaluateSum = $"sum([{expr}])".Compile();
            evaluateAvg = $"avg([{expr}])".Compile();
        }
        [Benchmark]
        public void Len()
        {
            var result = evaluateLen.EvaluateAs<int>();
            if (result != len)
                throw new Exception($"evaluate failed,result:{result},len:{len}");
        }
        [Benchmark]
        public void Max()
        {
            var result = evaluateMax.EvaluateAs<int>();
            if (result != max)
                throw new Exception($"evaluate failed,result:{result},max:{max}");
        }
        [Benchmark]
        public void Min()
        {
            var result = evaluateMin.EvaluateAs<int>();
            if (result != min)
                throw new Exception($"evaluate failed,result:{result},min:{min}");
        }
        [Benchmark]
        public void Sum()
        {
            var result = evaluateSum.EvaluateAs<int>();
            if (result != sum)
                throw new Exception($"evaluate failed,result:{result},sum:{sum}");
        }
        [Benchmark]
        public void Avg()
        {
            var result = evaluateAvg.EvaluateAs<decimal>();
            if (result != avg)
                throw new Exception($"evaluate failed,result:{result},avg:{avg}");
        }
    }
}
