using Extensions.Dictionary.Resolver;
using System;
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
        [InlineData(nameof(DefaultResolver))]
        [InlineData(nameof(DataContractResolver))]
        [InlineData(nameof(TextJsonResolver))]
        public void DisposeResolver_ObjectDisposedException(string resolverName)
        {
            var testType = typeof(DictionaryDummy);
            var resolver = GetResolver(resolverName);

            // Not disposed, should not throw an exception
            resolver.GetMemberInfos(testType);

            // Now disposed, exception should be thrown
            ((IDisposable)resolver).Dispose();
            ((IDisposable)resolver).Dispose();
            Assert.Throws<ObjectDisposedException>(() => resolver.GetMemberInfos(testType));
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
        [InlineData(nameof(TextJsonResolver))]
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
        [InlineData(nameof(TextJsonResolver))]
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
