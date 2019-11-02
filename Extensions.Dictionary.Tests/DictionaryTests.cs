using Extensions.Dictionary.Resolver;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
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
                { nameof(DictionaryDummy.Dict1), new Dictionary<string, object> { { "val1", 1 }, { "val2", "2" } } },
                { nameof(DictionaryDummy.Dict2), new Dictionary<string, object> { { "val1", 1 }, { "val2", 2 } } },
                { nameof(DictionaryDummy.Col1), new Dictionary<string, object> { { "0", 1 }, { "1", 2 } } },
                { nameof(DictionaryDummy.Vec3), new Dictionary<string, object> { { "X", 1f }, { "Y", 2f }, { "Z", 3f } } },
            };

            Assert.Equal(dictionary, expected, new DictionaryComparer<string, object>());

            var result = dictionary.ToInstance<DictionaryDummy>(new[] { new Vector3Converter() });
            Assert.Equal(dummy, result);

            var result2 = dictionary.ToInstance<DictionaryDummy>();
            Assert.Equal(new Vector3(), result2.Vec3);
        }

        [Theory]
        [InlineData(nameof(DefaultResolver))]
        [InlineData(nameof(DataContractResolver))]
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
        [InlineData(nameof(DefaultResolver))]
        [InlineData(nameof(DataContractResolver))]
        public void NullExceptions(string resolverName)
        {
            var resolver = GetResolver(resolverName);

            Assert.Equal(Array.Empty<MemberInfo>(), resolver.GetMemberInfos(null));

            var ex1 = Assert.Throws<ArgumentNullException>(() => resolver.GetPropertyName(null));
            Assert.Equal("memberInfo", ex1.ParamName);

            var ex2 = Assert.Throws<ArgumentNullException>(() => resolver.GetPropertyValue(null, null));
            Assert.Equal("memberInfo", ex2.ParamName);

            Assert.Throws<NotSupportedException>(() => resolver.GetPropertyValue(GetType().GetMethods()[0], null));
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
                { nameof(DictionaryDummy.Dict1), new Dictionary<string, object> { { "val1", 1 }, { "val2", "2" } } },
                { nameof(DictionaryDummy.Dict2), new Dictionary<string, object> { { "val1", 1 }, { "val2", 2 } } },
                { nameof(DictionaryDummy.Col1), new Dictionary<string, object> { { "0", 1 }, { "1", 2 } } },
                { nameof(DictionaryDummy.Vec3), new Dictionary<string, object> { { "X", 1f }, { "Y", 2f }, { "Z", 3f } } },
            };

            Assert.Equal(dictionary, expected, new DictionaryComparer<string, object>());

            var result = await dictionary.ToInstanceAsync<DictionaryDummy>(new[] { new Vector3Converter() }, default);
            Assert.Equal(dummy, result);

            var result2 = await dictionary.ToInstanceAsync<DictionaryDummy>();
            Assert.Equal(new Vector3(), result2.Vec3);
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
                { nameof(DictionaryDummy.Dict1), new Dictionary<string, object> { { "val1", 1 }, { "val2", "2" } } },
                { nameof(DictionaryDummy.Dict2), new Dictionary<string, object> { { "val1", 1 }, { "val2", 2 } } },
                { nameof(DictionaryDummy.Col1), new Dictionary<string, object> { { "0", 1 }, { "1", 2 } } },
                { nameof(DictionaryDummy.Vec3), new Dictionary<string, object> { { "X", 1f }, { "Y", 2f }, { "Z", 3f } } },
            };

            Assert.Equal(dictionary, expected, new DictionaryComparer<string, object>());

            var result = dictionary.ToInstance<DictionaryDummy>(new[] { new Vector3Converter() });
            Assert.Equal(dummy, result);

            var result2 = dictionary.ToInstance<DictionaryDummy>();
            Assert.Equal(new Vector3(), result2.Vec3);
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
                { nameof(DictionaryDummy.Dict1), new Dictionary<string, object> { { "val1", 1 }, { "val2", "2" } } },
                { nameof(DictionaryDummy.Dict2), new Dictionary<string, object> { { "val1", 1 }, { "val2", 2 } } },
                { nameof(DictionaryDummy.Col1), new Dictionary<string, object> { { "0", 1 }, { "1", 2 } } },
                { nameof(DictionaryDummy.Vec3), new Dictionary<string, object> { { "X", 1f }, { "Y", 2f }, { "Z", 3f } } },
            };

            Assert.Equal(expected, dictionary, new DictionaryComparer<string, object>());

            var result = await dictionary.ToInstanceAsync<DictionaryDummy>(new[] { new Vector3Converter() }, default);
            Assert.Equal(dummy, result);

            var result2 = await dictionary.ToInstanceAsync<DictionaryDummy>();
            Assert.Equal(new Vector3(), result2.Vec3);
        }
    }
}
