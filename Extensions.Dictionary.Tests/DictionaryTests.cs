using Extensions.Dictionary.Resolver;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Xunit;

namespace Extensions.Dictionary.Tests
{
    public class DictionaryTests : TestFixture
    {
        private readonly DictionaryDummy dummy;

        public DictionaryTests()
        {
            dummy = new DictionaryDummy
            {
                String01 = "A",
                //String02 = "B",
                //String03 = "C",
                String04 = "D",
                String05 = "E",
                String06 = "F",
                String08 = "H",
                Vec3 = new Vector3(3, 2, 1)
            };

            ((IList<int>)dummy.Col1).Add(3);
            dummy.Col2.Add(true);
            dummy.Dict1.Add("val3", true);
            dummy.Dict2.Add("val3", 3);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(nameof(DefaultResolver))]
        public void CreateDictionaryAndConvertItBack_Success(string resolverName)
        {
            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object>
            {
                { nameof(DictionaryDummy.String01), "A" },
                { nameof(DictionaryDummy.String02), nameof(DictionaryDummy.String02) },
                { nameof(DictionaryDummy.String03), nameof(DictionaryDummy.String03) },
                { nameof(DictionaryDummy.String04), "D" },
                { nameof(DictionaryDummy.String05), "E" },
                { nameof(DictionaryDummy.String06), "F" },
                // { nameof(Dummy.String07), nameof(Dummy.String07) },
                { nameof(DictionaryDummy.String08), "H" },
                { nameof(DictionaryDummy.Dict1), new Dictionary<string, object> { { "val1", 1 }, { "val2", "2" }, { "val3", true } } },
                { nameof(DictionaryDummy.Dict2), new Dictionary<string, object> { { "val1", 1 }, { "val2", 2 }, { "val3", 3 } } },
                { nameof(DictionaryDummy.Col1), new Dictionary<string, object> { { "0", 1 }, { "1", 2 }, { "2", 3 } } },
                { nameof(DictionaryDummy.Col2), new Dictionary<string, object> { { "0", 1 }, { "1", 2 }, { "2", "3" }, { "3", true } } },
                { nameof(DictionaryDummy.Vec3), new Dictionary<string, object> { { "X", 3f }, { "Y", 2f }, { "Z", 1f } } },
            };

            Assert.Equal(dictionary, expected, new DictionaryComparer<string, object>());

            var result = dictionary.ToInstance<DictionaryDummy>(resolver);
            Assert.Equal(dummy, result);

            var result2 = dictionary.ToInstance<DictionaryDummy>(resolver);
            Assert.Equal(new Vector3(3, 2, 1), result2.Vec3);
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

            var ex1 = Assert.Throws<ArgumentNullException>(() => resolver.GetMemberName(null));
            Assert.Equal("memberInfo", ex1.ParamName);

            var ex2 = Assert.Throws<ArgumentNullException>(() => resolver.GetMemberValue(null, null));
            Assert.Equal("memberInfo", ex2.ParamName);

            Assert.Throws<NotSupportedException>(() => resolver.GetMemberValue(GetType().GetMethods()[0], null));
        }

        [Theory]
        [InlineData(nameof(DataContractResolver))]
        [InlineData(nameof(JsonNetSerializerResolver))]
        public void CreateDictionaryAndConvertItBackAndRespectDataContracts_Success(string resolverName)
        {
            // Set to default, because this is going to be ignored!
            dummy.String08 = nameof(DictionaryDummy.String08);
            dummy.String05 = nameof(DictionaryDummy.String05);

            var resolver = GetResolver(resolverName);
            var dictionary = dummy.ToDictionary(resolver);
            var expected = new Dictionary<string, object>
            {
                { nameof(DictionaryDummy.String01), "A" },
                { nameof(DictionaryDummy.String02), nameof(DictionaryDummy.String02) },
                { nameof(DictionaryDummy.String03), nameof(DictionaryDummy.String03) },
                { "Custom" + nameof(DictionaryDummy.String04), "D" },
                // { nameof(Dummy.String05), nameof(Dummy.String05) },
                { nameof(DictionaryDummy.String06), "F" },
                //{ nameof(Dummy.String07), nameof(Dummy.String07) },
                // { nameof(Dummy.String08), nameof(Dummy.String08) },
                { nameof(DictionaryDummy.Dict1), new Dictionary<string, object> { { "val1", 1 }, { "val2", "2" }, { "val3", true } } },
                { nameof(DictionaryDummy.Dict2), new Dictionary<string, object> { { "val1", 1 }, { "val2", 2 }, { "val3", 3 } } },
                { nameof(DictionaryDummy.Col1), new Dictionary<string, object> { { "0", 1 }, { "1", 2 }, { "2", 3 } } },
                { nameof(DictionaryDummy.Col2), new Dictionary<string, object> { { "0", 1 }, { "1", 2 }, { "2", "3" }, { "3", true } } },
                { nameof(DictionaryDummy.Vec3), new Dictionary<string, object> { { "X", 3f }, { "Y", 2f }, { "Z", 1f } } },
            };

            Assert.Equal(dictionary, expected, new DictionaryComparer<string, object>());

            var result = dictionary.ToInstance<DictionaryDummy>(resolver);
            Assert.Equal(dummy, result);

            var result2 = dictionary.ToInstance<DictionaryDummy>(resolver);
            Assert.Equal(new Vector3(3, 2, 1), result2.Vec3);
        }
    }
}
