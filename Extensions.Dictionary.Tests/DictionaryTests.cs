using Extensions.Dictionary.Resolver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Extensions.Dictionary.Tests
{
    public class DictionaryTests
    {
        private static ISerializerResolver? GetResolver(string? resolverName)
        {
            return resolverName switch
            {
                nameof(DefaultResolver) => new DefaultResolver(),
                nameof(DataContractResolver) => new DataContractResolver(),
                nameof(JsonNetSerializerResolver) => new JsonNetSerializerResolver(),
                null => null,
                _ => throw new NotSupportedException($"Unsupported resolver ${resolverName}"),
            };
        }

        [Theory]
        [InlineData(null)]
        [InlineData(nameof(DefaultResolver))]
        public void CreateDictionaryAndConvertItBack_Success(string? resolverName)
        {
            var dummy = new Dummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object?>
            {
                { nameof(Dummy.String01), nameof(Dummy.String01) },
                { nameof(Dummy.String02), nameof(Dummy.String02) },
                { nameof(Dummy.String03), nameof(Dummy.String03) },
                { nameof(Dummy.String04), nameof(Dummy.String04) },
                { nameof(Dummy.String05), nameof(Dummy.String05) },
                { nameof(Dummy.String06), nameof(Dummy.String06) },
                // { nameof(Dummy.String07), nameof(Dummy.String07) },
                { nameof(Dummy.String08), nameof(Dummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = dictionary.ToInstance<Dummy>();
            Assert.Equal(dummy, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(nameof(DefaultResolver))]
        public async Task CreateDictionaryAndConvertItBackAsync_Success(string? resolverName)
        {
            var dummy = new Dummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object?>
            {
                { nameof(Dummy.String01), nameof(Dummy.String01) },
                { nameof(Dummy.String02), nameof(Dummy.String02) },
                { nameof(Dummy.String03), nameof(Dummy.String03) },
                { nameof(Dummy.String04), nameof(Dummy.String04) },
                { nameof(Dummy.String05), nameof(Dummy.String05) },
                { nameof(Dummy.String06), nameof(Dummy.String06) },
                // { nameof(Dummy.String07), nameof(Dummy.String07) },
                { nameof(Dummy.String08), nameof(Dummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = await dictionary.ToInstanceAsync<Dummy>(default);
            Assert.Equal(dummy, result);
        }

        [Theory]
        [InlineData(nameof(DataContractResolver))]
        [InlineData(nameof(JsonNetSerializerResolver))]
        public void CreateDictionaryAndConvertItBackAndRespectDataContracts_Success(string? resolverName)
        {
            var dummy = new Dummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object?>
            {
                { nameof(Dummy.String01), nameof(Dummy.String01) },
                { nameof(Dummy.String02), nameof(Dummy.String02) },
                { nameof(Dummy.String03), nameof(Dummy.String03) },
                { "Custom" + nameof(Dummy.String04), nameof(Dummy.String04) },
                // { nameof(Dummy.String05), nameof(Dummy.String05) },
                { nameof(Dummy.String06), nameof(Dummy.String06) },
                //{ nameof(Dummy.String07), nameof(Dummy.String07) },
                //{ nameof(Dummy.String08), nameof(Dummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = dictionary.ToInstance<Dummy>();
            Assert.Equal(dummy, result);
        }

        [Theory]
        [InlineData(nameof(DataContractResolver))]
        [InlineData(nameof(JsonNetSerializerResolver))]
        public async Task CreateDictionaryAndConvertItBackAndRespectDataContractsAsync_Success(string? resolverName)
        {
            var dummy = new Dummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object?>
            {
                { nameof(Dummy.String01), nameof(Dummy.String01) },
                { nameof(Dummy.String02), nameof(Dummy.String02) },
                { nameof(Dummy.String03), nameof(Dummy.String03) },
                { "Custom" + nameof(Dummy.String04), nameof(Dummy.String04) },
                // { nameof(Dummy.String05), nameof(Dummy.String05) },
                { nameof(Dummy.String06), nameof(Dummy.String06) },
                //{ nameof(Dummy.String07), nameof(Dummy.String07) },
                //{ nameof(Dummy.String08), nameof(Dummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = await dictionary.ToInstanceAsync<Dummy>(default);
            Assert.Equal(dummy, result);
        }
    }
}
