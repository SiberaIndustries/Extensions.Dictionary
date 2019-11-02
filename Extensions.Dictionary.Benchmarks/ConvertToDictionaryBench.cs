using BenchmarkDotNet.Attributes;
using Extensions.Dictionary.Resolver;
using Extensions.Dictionary.Tests;

namespace Extensions.Dictionary.Benchmarks
{
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
