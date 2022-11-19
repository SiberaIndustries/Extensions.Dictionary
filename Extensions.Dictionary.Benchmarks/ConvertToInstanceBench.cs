using BenchmarkDotNet.Attributes;
using Extensions.Dictionary.Resolver;
using Extensions.Dictionary.Tests;
using Newtonsoft.Json;

namespace Extensions.Dictionary.Benchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [RankColumn]
    [BenchmarkCategory(nameof(DictionaryExtensions.ToInstance))]
    public class ConvertToInstanceBench
    {
        private readonly IDictionary<string, object?> dummy = new DictionaryDummy().ToDictionary();
        private readonly ConverterSettings defaultResolver = new() { Resolver = new DefaultResolver() };
        private readonly ConverterSettings dataContractResolver = new() { Resolver = new DataContractResolver() };
        private readonly ConverterSettings dataContractResolverIgnoreAncestors = new() { Resolver = new DataContractResolver { InspectAncestors = false } };
        private readonly ConverterSettings jsonResolver = new() { Resolver = new JsonNetSerializerResolver() };

        [Params(1, 10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var d = (IDictionary<string, object>)((IDictionary<string, object>)dummy)[nameof(DictionaryDummy.Dict1)];
            for (int i = 1; i < N; i++)
            {
                d[nameof(N)] = new DictionaryDummy().ToDictionary();
                d = (IDictionary<string, object>)d[nameof(N)];
            }
        }

        [Benchmark(Baseline = true)]
        public void DefaultResolver() =>
            dummy.ToInstance<DictionaryDummy>(defaultResolver);

        [Benchmark]
        public void DataContractResolver() =>
            dummy.ToInstance<DictionaryDummy>(dataContractResolver);

        [Benchmark]
        public void DataContractResolverIgnoreAncestors() =>
            dummy.ToInstance<DictionaryDummy>(dataContractResolverIgnoreAncestors);

        [Benchmark]
        public void JsonNetResolver() =>
            dummy.ToInstance<DictionaryDummy>(jsonResolver);

        [Benchmark]
        public void PureJsonNet() =>
            JsonConvert.DeserializeObject<DictionaryDummy>(JsonConvert.SerializeObject(dummy));
    }
}
