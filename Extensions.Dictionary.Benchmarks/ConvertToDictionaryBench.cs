using BenchmarkDotNet.Attributes;
using Extensions.Dictionary.Resolver;
using Extensions.Dictionary.Tests;

namespace Extensions.Dictionary.Benchmarks
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [RankColumn]
    [BenchmarkCategory("ToDictionary")]
    public class ConvertToDictionaryBench
    {
        private readonly DictionaryDummy dummy = new DictionaryDummy();
        private readonly ISerializerResolver defaultResolver = new DefaultResolver();
        private readonly ISerializerResolver dataContractResolver = new DataContractResolver { InspectAncestors = false };
        private readonly ISerializerResolver jsonResolver = new JsonNetSerializerResolver();

        [Benchmark(Baseline = true)]
        public void DefaultResolver() =>
            dummy.ToDictionary(defaultResolver);

        [Benchmark]
        public void DataContractResolver() =>
            dummy.ToDictionary(dataContractResolver);

        [Benchmark]
        public void JsonNetResolver() =>
            dummy.ToDictionary(jsonResolver);
    }
}
