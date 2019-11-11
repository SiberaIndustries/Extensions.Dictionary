using System;
using Xunit;

namespace Extensions.Dictionary.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void SimpleTypes_True()
        {
            var types = new Type[]
            {
                typeof(string),
                typeof(int),
                typeof(float),
                typeof(double),
                typeof(long),
            };

            Assert.All(types, x => Assert.True(x.IsSimpleType()));
        }

        [Fact]
        public void NoSimpleTypes_False()
        {
            var types = new Type[]
            {
                typeof(DictionaryDummy),
            };

            Assert.All(types, x => Assert.False(x.IsSimpleType()));
        }
    }
}
