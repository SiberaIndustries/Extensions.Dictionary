using Extensions.Dictionary.Resolver;
using System;
using Xunit;
using static Extensions.Dictionary.Tests.MemberInfoDummy;

namespace Extensions.Dictionary.Tests
{
    public class MemberInfoExtensionsTests : TestFixture
    {
        [Fact]
        public void IsSimpleTypeOnUnkownMembers_ArgumentNullException()
        {
            var dummyType = typeof(MemberInfoDummy);

            var invalidPropInfo = dummyType.GetProperty("InvalidPropertyName");
            var invalidFieldInfo = dummyType.GetField("InvalidFieldName");

            var ex1 = Assert.Throws<ArgumentNullException>(() => invalidPropInfo.IsSimpleType());
            Assert.Equal("memberInfo", ex1.ParamName);

            var ex2 = Assert.Throws<ArgumentNullException>(() => invalidFieldInfo.IsSimpleType());
            Assert.Equal("memberInfo", ex2.ParamName);
        }

        [Fact]
        public void IsSimpleTypeOnUnkownMemberTypes_NotSupportedException()
        {
            var dummyType = typeof(MemberInfoDummy);

            var invalidPropInfo = dummyType.GetMethod(nameof(MemberInfoDummy.Method1));

            var ex = Assert.Throws<NotSupportedException>(() => invalidPropInfo.IsSimpleType());
            Assert.Equal("MemberType Method not supported", ex.Message);
        }

        [Fact]
        public void IsSimpleType_Success()
        {
            var dummyType = typeof(MemberInfoDummy);

            var value1Info = dummyType.GetProperty(nameof(MemberInfoDummy.Value1));
            var value2Info = dummyType.GetField(nameof(MemberInfoDummy.Value2));
            var value3Info = dummyType.GetProperty(nameof(MemberInfoDummy.Value3));
            var value4Info = dummyType.GetProperty(nameof(MemberInfoDummy.Value4));
            var subDummyInfo = dummyType.GetProperty(nameof(MemberInfoDummy.SubDummy));

            Assert.True(value1Info.IsSimpleType());
            Assert.True(value2Info.IsSimpleType());
            Assert.True(value3Info.IsSimpleType());
            Assert.True(value4Info.IsSimpleType());
            Assert.False(subDummyInfo.IsSimpleType());
        }

        [Fact]
        public void GetNameWithDefaultResolver_Success()
        {
            var resolver = new DefaultResolver();
            var dummyType = typeof(MemberInfoDummy);

            var value1Name = dummyType.GetProperty(nameof(MemberInfoDummy.Value1)).GetName(resolver);
            var value2Name = dummyType.GetField(nameof(MemberInfoDummy.Value2)).GetName(resolver);
            var value3Name = dummyType.GetProperty(nameof(MemberInfoDummy.Value3)).GetName(resolver);
            var value4Name = dummyType.GetProperty(nameof(MemberInfoDummy.Value4)).GetName(resolver);
            var subDummyName = dummyType.GetProperty(nameof(MemberInfoDummy.SubDummy)).GetName(resolver);

            Assert.Equal(nameof(MemberInfoDummy.Value1), value1Name);
            Assert.Equal(nameof(MemberInfoDummy.Value2), value2Name);
            Assert.Equal(nameof(MemberInfoDummy.Value3), value3Name);
            Assert.Equal(nameof(MemberInfoDummy.Value4), value4Name);
            Assert.Equal(nameof(MemberInfoDummy.SubDummy), subDummyName);
        }

        [Fact]
        public void GetNameWithDataContractResolver_Success()
        {
            var resolver = new DataContractResolver();
            var dummyType = typeof(MemberInfoDummy);

            var value1PropName = dummyType.GetProperty(nameof(MemberInfoDummy.Value1)).GetName(resolver);
            var value2PropName = dummyType.GetField(nameof(MemberInfoDummy.Value2)).GetName(resolver);
            var value3PropName = dummyType.GetProperty(nameof(MemberInfoDummy.Value3)).GetName(resolver);
            var value4PropName = dummyType.GetProperty(nameof(MemberInfoDummy.Value4)).GetName(resolver);
            var subDummyPropName = dummyType.GetProperty(nameof(MemberInfoDummy.SubDummy)).GetName(resolver);

            Assert.Equal(nameof(MemberInfoDummy.Value1), value1PropName);
            Assert.Equal(nameof(MemberInfoDummy.Value2), value2PropName);
            Assert.Equal("Value33", value3PropName);
            Assert.Equal("Value44", value4PropName);
            Assert.Equal(nameof(MemberInfoDummy.SubDummy), subDummyPropName);
        }

        [Theory]
        [InlineData(null, nameof(DefaultResolver))]
        [InlineData("", nameof(DefaultResolver))]
        [InlineData("foobar", nameof(DefaultResolver))]
        [InlineData(null, nameof(DataContractResolver))]
        [InlineData("", nameof(DataContractResolver))]
        [InlineData("foobar", nameof(DataContractResolver))]
        public void GetValueWithDataContractResolver_Success(string value, string resolverName)
        {
            var resolver = GetResolver(resolverName);
            var dummyType = typeof(MemberInfoDummy);
            var subDummy = new MemberInfoSubDummy();
            var dummy = new MemberInfoDummy
            {
                Value1 = value,
                Value2 = value,
                Value3 = value,
                Value4 = value,
                SubDummy = subDummy
            };

            var value1MemberValue = dummyType.GetProperty(nameof(MemberInfoDummy.Value1)).GetValue(dummy, resolver);
            var value2MemberValue = dummyType.GetField(nameof(MemberInfoDummy.Value2)).GetValue(dummy, resolver);
            var value3MemberValue = dummyType.GetProperty(nameof(MemberInfoDummy.Value3)).GetValue(dummy, resolver);
            var value4MemberValue = dummyType.GetProperty(nameof(MemberInfoDummy.Value4)).GetValue(dummy, resolver);
            var subDummyMemberValue = dummyType.GetProperty(nameof(MemberInfoDummy.SubDummy)).GetValue(dummy, resolver);

            Assert.Equal(value, value1MemberValue);
            Assert.Equal(value, value2MemberValue);
            Assert.Equal(value, value3MemberValue);
            Assert.Equal(value, value4MemberValue);
            Assert.Equal(subDummy, subDummyMemberValue);
        }
    }
}
