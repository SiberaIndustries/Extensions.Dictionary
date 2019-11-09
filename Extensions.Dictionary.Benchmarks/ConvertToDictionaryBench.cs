using BenchmarkDotNet.Attributes;
using Extensions.Dictionary.Resolver;
using Extensions.Dictionary.Tests;

namespace Extensions.Dictionary.Benchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [RankColumn]
    [BenchmarkCategory(nameof(ObjectExtensions.ToDictionary))]
    public class ConvertToDictionaryBench
    {
        private readonly DictionaryDummy dummy = new DictionaryDummy();
        private readonly ISerializerResolver defaultResolver = new DefaultResolver();
        private readonly ISerializerResolver dataContractResolver = new DataContractResolver();
        private readonly ISerializerResolver dataContractResolverIgnoreAncestors = new DataContractResolver { InspectAncestors = false };
        private readonly ISerializerResolver jsonResolver = new JsonNetSerializerResolver();

        [Params(1, 10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var d = dummy;
            for (int i = 1; i < N; i++)
            {
                var val = new DictionaryDummy();
                d.Dict1[nameof(N)] = val;
                d = val;
            }
        }

        [Benchmark(Baseline = true)]
        public void DefaultResolver() =>
            dummy.ToDictionary(defaultResolver);

        [Benchmark]
        public void DataContractResolver() =>
            dummy.ToDictionary(dataContractResolver);

        [Benchmark]
        public void DataContractResolverIgnoreAncestors() =>
            dummy.ToDictionary(dataContractResolverIgnoreAncestors);

        [Benchmark]
        public void JsonNetResolver() =>
            dummy.ToDictionary(jsonResolver);
    }
}
