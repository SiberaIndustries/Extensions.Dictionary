using Extensions.Dictionary.Internal;
using System;
using System.Numerics;
using Xunit;

namespace Extensions.Dictionary.Tests.Internal
{
    public class UtilitiesTests
    {
        private enum DummyEnum : ushort
        {
            ONE
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsSimpleType_Success(bool allowNullables)
        {
            Assert.True(typeof(char).IsSimpleType(allowNullables));
            Assert.True(typeof(bool).IsSimpleType(allowNullables));
            Assert.True(typeof(sbyte).IsSimpleType(allowNullables));
            Assert.True(typeof(short).IsSimpleType(allowNullables));
            Assert.True(typeof(ushort).IsSimpleType(allowNullables));
            Assert.True(typeof(int).IsSimpleType(allowNullables));
            Assert.True(typeof(byte).IsSimpleType(allowNullables));
            Assert.True(typeof(uint).IsSimpleType(allowNullables));
            Assert.True(typeof(long).IsSimpleType(allowNullables));
            Assert.True(typeof(ulong).IsSimpleType(allowNullables));
            Assert.True(typeof(float).IsSimpleType(allowNullables));
            Assert.True(typeof(double).IsSimpleType(allowNullables));
            Assert.True(typeof(decimal).IsSimpleType(allowNullables));
            Assert.True(typeof(BigInteger).IsSimpleType(allowNullables));

            Assert.Equal(allowNullables, typeof(char?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(bool?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(sbyte?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(short?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(ushort?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(int?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(byte?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(uint?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(long?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(ulong?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(float?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(double?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(decimal?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(BigInteger?).IsSimpleType(allowNullables));
            Assert.Equal(allowNullables, typeof(string).IsSimpleType(allowNullables));

            Assert.Equal(allowNullables, Utilities.IsSimpleType(null, allowNullables));

            Assert.False(typeof(object).IsSimpleType(allowNullables));
        }

        [Fact]
        public void IsNummeric_True()
        {
            Assert.True(Utilities.IsNummeric((sbyte)1));
            Assert.True(Utilities.IsNummeric((byte)1));
            Assert.True(Utilities.IsNummeric((short)1));
            Assert.True(Utilities.IsNummeric((ushort)1));
            Assert.True(Utilities.IsNummeric(1));
            Assert.True(Utilities.IsNummeric((uint)1));
            Assert.True(Utilities.IsNummeric((long)1));
            Assert.True(Utilities.IsNummeric((ulong)1));

            Assert.False(Utilities.IsNummeric("1"));
            Assert.False(Utilities.IsNummeric(true));
        }

        [Fact]
        public void GetTypeCode_AlwaysEqual()
        {
            Assert.Equal(Utilities.PrimitiveTypeCode.UInt16, Utilities.GetTypeCode(typeof(ushort)));
            Assert.Equal(Utilities.PrimitiveTypeCode.UInt16, Utilities.GetTypeCode(typeof(DummyEnum)));

            Assert.Equal(Utilities.PrimitiveTypeCode.UInt16Nullable, Utilities.GetTypeCode(typeof(ushort?)));
            Assert.Equal(Utilities.PrimitiveTypeCode.UInt16Nullable, Utilities.GetTypeCode(typeof(DummyEnum?)));
        }

        [Fact]
        public void ToBigInteger_AlwaysEqual()
        {
            var bigInt = new BigInteger(42);

            Assert.Equal(bigInt, Utilities.ToBigInteger(new BigInteger(42)));
            Assert.Equal(bigInt, Utilities.ToBigInteger("42"));
            Assert.Equal(bigInt, Utilities.ToBigInteger(42f));
            Assert.Equal(bigInt, Utilities.ToBigInteger(42.0));
            Assert.Equal(bigInt, Utilities.ToBigInteger(new decimal(42)));
            Assert.Equal(bigInt, Utilities.ToBigInteger(42));
            Assert.Equal(bigInt, Utilities.ToBigInteger(42L));
            Assert.Equal(bigInt, Utilities.ToBigInteger((uint)42));
            Assert.Equal(bigInt, Utilities.ToBigInteger((ulong)42));
            Assert.Equal(bigInt, Utilities.ToBigInteger(bigInt.ToByteArray()));
        }

        [Fact]
        public void ToBigIntegerWithInvalidValues_Exception()
        {
            Assert.Throws<InvalidCastException>(() => Utilities.ToBigInteger(new Vector3(1, 2, 3)));
            Assert.Throws<InvalidCastException>(() => Utilities.ToBigInteger(null));
        }

        [Fact]
        public void FromBigInteger_AlwaysEqual()
        {
            var bigInt = new BigInteger(42);
            object result;

            result = Utilities.FromBigInteger(bigInt, typeof(decimal));
            Assert.IsType<decimal>(result);
            Assert.Equal(new decimal(42), result);

            result = Utilities.FromBigInteger(bigInt, typeof(double));
            Assert.IsType<double>(result);
            Assert.Equal(42.0, result);

            result = Utilities.FromBigInteger(bigInt, typeof(float));
            Assert.IsType<float>(result);
            Assert.Equal(42f, result);

            result = Utilities.FromBigInteger(bigInt, typeof(ulong));
            Assert.IsType<ulong>(result);
            Assert.Equal((ulong)42, result);

            result = Utilities.FromBigInteger(bigInt, typeof(bool));
            Assert.IsType<bool>(result);
            Assert.Equal(true, result);

            result = Utilities.FromBigInteger(bigInt, typeof(int));
            Assert.IsType<int>(result);
            Assert.Equal(42, result);
        }

        [Fact]
        public void TryGetNonNullableType_Success()
        {
            var result = typeof(float).TryGetNonNullableType(out Type t);
            Assert.False(result);
            Assert.Null(t);

            result = typeof(float?).TryGetNonNullableType(out t);
            Assert.True(result);
            Assert.NotNull(t);
        }
    }
}
