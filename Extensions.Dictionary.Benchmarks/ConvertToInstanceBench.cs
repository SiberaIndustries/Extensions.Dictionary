﻿using BenchmarkDotNet.Attributes;
using Extensions.Dictionary.Resolver;
using Extensions.Dictionary.Tests;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Extensions.Dictionary.Benchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [RankColumn]
    [BenchmarkCategory(nameof(DictionaryExtensions.ToInstance))]
    public class ConvertToInstanceBench
    {
        private readonly IDictionary<string, object?> dummy = new DictionaryDummy().ToDictionary();
        private readonly ISerializerResolver defaultResolver = new DefaultResolver();
        private readonly ISerializerResolver dataContractResolver = new DataContractResolver();
        private readonly ISerializerResolver dataContractResolverIgnoreAncestors = new DataContractResolver { InspectAncestors = false };
        private readonly ISerializerResolver jsonResolver = new JsonNetSerializerResolver();

        [Params(1, 10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var d = (IDictionary<string, object>)dummy;
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
