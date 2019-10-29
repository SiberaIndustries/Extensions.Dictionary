using Extensions.Dictionary.Resolver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Extensions.Dictionary.Tests
{
    public class DictionaryTests : TestFixture
    {
        [Theory]
        [InlineData(null)]
        [InlineData(nameof(DefaultResolver))]
        public void CreateDictionaryAndConvertItBack_Success(string resolverName)
        {
            var dummy = new DictionaryDummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object>
            {
                { nameof(DictionaryDummy.String01), nameof(DictionaryDummy.String01) },
                { nameof(DictionaryDummy.String02), nameof(DictionaryDummy.String02) },
                { nameof(DictionaryDummy.String03), nameof(DictionaryDummy.String03) },
                { nameof(DictionaryDummy.String04), nameof(DictionaryDummy.String04) },
                { nameof(DictionaryDummy.String05), nameof(DictionaryDummy.String05) },
                { nameof(DictionaryDummy.String06), nameof(DictionaryDummy.String06) },
                // { nameof(Dummy.String07), nameof(Dummy.String07) },
                { nameof(DictionaryDummy.String08), nameof(DictionaryDummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = dictionary.ToInstance<DictionaryDummy>();
            Assert.Equal(dummy, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(nameof(DefaultResolver))]
        public async Task CreateDictionaryAndConvertItBackAsync_Success(string resolverName)
        {
            var dummy = new DictionaryDummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object>
            {
                { nameof(DictionaryDummy.String01), nameof(DictionaryDummy.String01) },
                { nameof(DictionaryDummy.String02), nameof(DictionaryDummy.String02) },
                { nameof(DictionaryDummy.String03), nameof(DictionaryDummy.String03) },
                { nameof(DictionaryDummy.String04), nameof(DictionaryDummy.String04) },
                { nameof(DictionaryDummy.String05), nameof(DictionaryDummy.String05) },
                { nameof(DictionaryDummy.String06), nameof(DictionaryDummy.String06) },
                // { nameof(Dummy.String07), nameof(Dummy.String07) },
                { nameof(DictionaryDummy.String08), nameof(DictionaryDummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = await dictionary.ToInstanceAsync<DictionaryDummy>(default);
            Assert.Equal(dummy, result);
        }

        [Theory]
        [InlineData(nameof(DataContractResolver))]
        [InlineData(nameof(JsonNetSerializerResolver))]
        public void CreateDictionaryAndConvertItBackAndRespectDataContracts_Success(string resolverName)
        {
            var dummy = new DictionaryDummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object>
            {
                { nameof(DictionaryDummy.String01), nameof(DictionaryDummy.String01) },
                { nameof(DictionaryDummy.String02), nameof(DictionaryDummy.String02) },
                { nameof(DictionaryDummy.String03), nameof(DictionaryDummy.String03) },
                { "Custom" + nameof(DictionaryDummy.String04), nameof(DictionaryDummy.String04) },
                // { nameof(Dummy.String05), nameof(Dummy.String05) },
                { nameof(DictionaryDummy.String06), nameof(DictionaryDummy.String06) },
                //{ nameof(Dummy.String07), nameof(Dummy.String07) },
                //{ nameof(Dummy.String08), nameof(Dummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = dictionary.ToInstance<DictionaryDummy>();
            Assert.Equal(dummy, result);
        }

        [Theory]
        [InlineData(nameof(DataContractResolver))]
        [InlineData(nameof(JsonNetSerializerResolver))]
        public async Task CreateDictionaryAndConvertItBackAndRespectDataContractsAsync_Success(string resolverName)
        {
            var dummy = new DictionaryDummy();
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object>
            {
                { nameof(DictionaryDummy.String01), nameof(DictionaryDummy.String01) },
                { nameof(DictionaryDummy.String02), nameof(DictionaryDummy.String02) },
                { nameof(DictionaryDummy.String03), nameof(DictionaryDummy.String03) },
                { "Custom" + nameof(DictionaryDummy.String04), nameof(DictionaryDummy.String04) },
                // { nameof(Dummy.String05), nameof(Dummy.String05) },
                { nameof(DictionaryDummy.String06), nameof(DictionaryDummy.String06) },
                //{ nameof(Dummy.String07), nameof(Dummy.String07) },
                //{ nameof(Dummy.String08), nameof(Dummy.String08) },
            };

            Assert.Equal(dictionary, expected);

            var result = await dictionary.ToInstanceAsync<DictionaryDummy>(default);
            Assert.Equal(dummy, result);
        }
    }
}
