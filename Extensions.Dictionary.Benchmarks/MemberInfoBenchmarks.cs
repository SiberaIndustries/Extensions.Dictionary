using BenchmarkDotNet.Attributes;
using Extensions.Dictionary.Tests;
using System.Reflection;

namespace Extensions.Dictionary.Benchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [RankColumn]
    [BenchmarkCategory(nameof(MemberInfoExtensions))]
    [DisassemblyDiagnoser(1, true, false, true, false, false, true)]
    public class MemberInfoTypeCastBench
    {
        [Params(1000)]
        public int N;

        private readonly DictionaryDummy dummy = new DictionaryDummy();
        private readonly MemberInfo memberInfo = typeof(DictionaryDummy).GetProperty(nameof(DictionaryDummy.String01))!;

        [Benchmark(Baseline = true)]
        public object? Cast()
        {
            object? val = null;
            for (int i = 0; i < N; i++)
            {
                val = ((PropertyInfo)memberInfo).GetValue(dummy);
            }

            return val;
        }

        [Benchmark]
        public object? As()
        {
            object? val = null;
            for (int i = 0; i < N; i++)
            {
                val = (memberInfo as PropertyInfo)?.GetValue(dummy);
            }

            return val;
        }

        [Benchmark]
        public object? Is()
        {
            object? val = null;
            for (int i = 0; i < N; i++)
            {
                if (memberInfo is PropertyInfo pi)
                {
                    val = pi.GetValue(dummy);
                }
            }

            return val;
        }
    }
}
