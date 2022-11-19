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
        private readonly DictionaryDummy dummy = new();
        private readonly ConverterSettings defaultResolver = new() { Resolver = new DefaultResolver() };
        private readonly ConverterSettings dataContractResolver = new() { Resolver = new DataContractResolver() };
        private readonly ConverterSettings dataContractResolverIgnoreAncestors = new() { Resolver = new DataContractResolver { InspectAncestors = false } };
        private readonly ConverterSettings jsonResolver = new() { Resolver = new JsonNetSerializerResolver() };

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
