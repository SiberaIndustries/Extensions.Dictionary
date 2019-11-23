using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace Extensions.Dictionary.Internal
{
    internal static class Utilities
    {
        public static readonly Dictionary<Type, PrimitiveTypeCode> TypeCodeMap = new Dictionary<Type, PrimitiveTypeCode>(40)
        {
            { typeof(char), PrimitiveTypeCode.Char },
            { typeof(char?), PrimitiveTypeCode.CharNullable },
            { typeof(bool), PrimitiveTypeCode.Boolean },
            { typeof(bool?), PrimitiveTypeCode.BooleanNullable },
            { typeof(sbyte), PrimitiveTypeCode.SByte },
            { typeof(sbyte?), PrimitiveTypeCode.SByteNullable },
            { typeof(short), PrimitiveTypeCode.Int16 },
            { typeof(short?), PrimitiveTypeCode.Int16Nullable },
            { typeof(ushort), PrimitiveTypeCode.UInt16 },
            { typeof(ushort?), PrimitiveTypeCode.UInt16Nullable },
            { typeof(int), PrimitiveTypeCode.Int32 },
            { typeof(int?), PrimitiveTypeCode.Int32Nullable },
            { typeof(byte), PrimitiveTypeCode.Byte },
            { typeof(byte?), PrimitiveTypeCode.ByteNullable },
            { typeof(uint), PrimitiveTypeCode.UInt32 },
            { typeof(uint?), PrimitiveTypeCode.UInt32Nullable },
            { typeof(long), PrimitiveTypeCode.Int64 },
            { typeof(long?), PrimitiveTypeCode.Int64Nullable },
            { typeof(ulong), PrimitiveTypeCode.UInt64 },
            { typeof(ulong?), PrimitiveTypeCode.UInt64Nullable },
            { typeof(float), PrimitiveTypeCode.Single },
            { typeof(float?), PrimitiveTypeCode.SingleNullable },
            { typeof(double), PrimitiveTypeCode.Double },
            { typeof(double?), PrimitiveTypeCode.DoubleNullable },
            { typeof(DateTime), PrimitiveTypeCode.DateTime },
            { typeof(DateTime?), PrimitiveTypeCode.DateTimeNullable },
            { typeof(DateTimeOffset), PrimitiveTypeCode.DateTimeOffset },
            { typeof(DateTimeOffset?), PrimitiveTypeCode.DateTimeOffsetNullable },
            { typeof(decimal), PrimitiveTypeCode.Decimal },
            { typeof(decimal?), PrimitiveTypeCode.DecimalNullable },
            { typeof(Guid), PrimitiveTypeCode.Guid },
            { typeof(Guid?), PrimitiveTypeCode.GuidNullable },
            { typeof(TimeSpan), PrimitiveTypeCode.TimeSpan },
            { typeof(TimeSpan?), PrimitiveTypeCode.TimeSpanNullable },
            { typeof(BigInteger), PrimitiveTypeCode.BigInteger },
            { typeof(BigInteger?), PrimitiveTypeCode.BigIntegerNullable },
            { typeof(Uri), PrimitiveTypeCode.Uri },
            { typeof(string), PrimitiveTypeCode.String },
            { typeof(byte[]), PrimitiveTypeCode.Bytes },
            { typeof(DBNull), PrimitiveTypeCode.DBNull }
        };

        public enum PrimitiveTypeCode
        {
            Empty = 0,
            Object = 1,
            Char = 2,
            CharNullable = 3,
            Boolean = 4,
            BooleanNullable = 5,
            SByte = 6,
            SByteNullable = 7,
            Int16 = 8,
            Int16Nullable = 9,
            UInt16 = 10,
            UInt16Nullable = 11,
            Int32 = 12,
            Int32Nullable = 13,
            Byte = 14,
            ByteNullable = 15,
            UInt32 = 16,
            UInt32Nullable = 17,
            Int64 = 18,
            Int64Nullable = 19,
            UInt64 = 20,
            UInt64Nullable = 21,
            Single = 22,
            SingleNullable = 23,
            Double = 24,
            DoubleNullable = 25,
            DateTime = 26,
            DateTimeNullable = 27,
            DateTimeOffset = 28,
            DateTimeOffsetNullable = 29,
            Decimal = 30,
            DecimalNullable = 31,
            Guid = 32,
            GuidNullable = 33,
            TimeSpan = 34,
            TimeSpanNullable = 35,
            BigInteger = 36,
            BigIntegerNullable = 37,
            Uri = 38,
            String = 39,
            Bytes = 40,
            DBNull = 41
        }

        public static bool IsSimpleType(this Type type, bool allowNullables = true)
        {
            if (type == null)
            {
                return allowNullables;
            }

            var code = GetTypeCode(type);
            switch (code)
            {
                case PrimitiveTypeCode.Char:
                case PrimitiveTypeCode.Boolean:
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                case PrimitiveTypeCode.Single:
                case PrimitiveTypeCode.Double:
                case PrimitiveTypeCode.Decimal:
                case PrimitiveTypeCode.BigInteger:
                    return true;
                case PrimitiveTypeCode.CharNullable:
                case PrimitiveTypeCode.BooleanNullable:
                case PrimitiveTypeCode.SByteNullable:
                case PrimitiveTypeCode.Int16Nullable:
                case PrimitiveTypeCode.UInt16Nullable:
                case PrimitiveTypeCode.Int32Nullable:
                case PrimitiveTypeCode.ByteNullable:
                case PrimitiveTypeCode.UInt32Nullable:
                case PrimitiveTypeCode.Int64Nullable:
                case PrimitiveTypeCode.UInt64Nullable:
                case PrimitiveTypeCode.SingleNullable:
                case PrimitiveTypeCode.DoubleNullable:
                case PrimitiveTypeCode.DecimalNullable:
                case PrimitiveTypeCode.BigIntegerNullable:
                case PrimitiveTypeCode.String:
                    return allowNullables;
                default:
                    return false;
            }
        }

        public static bool TryGetUnderlyingType(this Type type, out Type underlyingType)
        {
            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }

        public static PrimitiveTypeCode GetTypeCode(Type type)
        {
            if (TypeCodeMap.TryGetValue(type, out PrimitiveTypeCode typeCode))
            {
                return typeCode;
            }
            else if (type.IsEnum)
            {
                return GetTypeCode(Enum.GetUnderlyingType(type));
            }

            if (type.TryGetUnderlyingType(out Type nonNullable) && nonNullable.IsEnum)
            {
                var nullableUnderlyingType = typeof(Nullable<>).MakeGenericType(new[] { Enum.GetUnderlyingType(nonNullable) });
                return GetTypeCode(nullableUnderlyingType);
            }

            return PrimitiveTypeCode.Object;
        }

        public static object? ConVal(this object? initialValue, Type targetType, ConverterSettings settings) =>
            TryConvertValue(initialValue, targetType, settings, out object? value)
                ? value
                : throw new InvalidOperationException();

        public static T ConVal<T>(this object? initialValue, ConverterSettings settings)
            where T : struct
        {
            var value = ConVal(initialValue, typeof(T), settings);
            return value == null ? default : (T)value;
        }

        public static bool TryConvertValue(this object? initialValue, Type targetType, ConverterSettings settings, out object? value)
        {
            if (initialValue == null)
            {
                throw new ArgumentNullException(nameof(initialValue));
            }

            if (targetType.TryGetUnderlyingType(out Type nonNullableType))
            {
                targetType = nonNullableType;
            }

            var initialType = initialValue.GetType();
            if (targetType == initialType)
            {
                value = initialValue;
                return true;
            }

            // use Convert.ChangeType if both types are IConvertible
            if (typeof(IConvertible).IsAssignableFrom(initialType) && typeof(IConvertible).IsAssignableFrom(targetType))
            {
                if (targetType.IsEnum)
                {
                    if (initialValue is string)
                    {
                        value = Enum.Parse(targetType, initialValue.ToString(), true);
                        return true;
                    }
                    else if (IsNummeric(initialValue))
                    {
                        value = Enum.ToObject(targetType, initialValue);
                        return true;
                    }
                }

                value = Convert.ChangeType(initialValue, targetType, settings.Culture);
                return true;
            }

            if (initialValue is DateTime dt && targetType == typeof(DateTimeOffset))
            {
                value = new DateTimeOffset(dt);
                return true;
            }

            if (initialValue is byte[] bytes && targetType == typeof(Guid))
            {
                value = new Guid(bytes);
                return true;
            }

            if (initialValue is Guid guid && targetType == typeof(byte[]))
            {
                value = guid.ToByteArray();
                return true;
            }

            if (initialValue is string s)
            {
                if (targetType == typeof(Guid))
                {
                    value = new Guid(s);
                    return true;
                }

                if (targetType == typeof(Uri))
                {
                    value = new Uri(s, UriKind.RelativeOrAbsolute);
                    return true;
                }

                if (targetType == typeof(TimeSpan))
                {
                    value = TimeSpan.Parse(s, CultureInfo.InvariantCulture);
                    return true;
                }

                if (targetType == typeof(byte[]))
                {
                    value = Convert.FromBase64String(s);
                    return true;
                }

                if (targetType == typeof(Version))
                {
                    if (Version.TryParse(s, out Version? result))
                    {
                        value = result;
                        return true;
                    }

                    value = null;
                    return false;
                }

                if (typeof(Type).IsAssignableFrom(targetType))
                {
                    value = Type.GetType(s, true);
                    return true;
                }
            }

            if (targetType == typeof(BigInteger))
            {
                value = ToBigInteger(initialValue);
                return true;
            }

            if (initialValue is BigInteger integer)
            {
                value = FromBigInteger(integer, targetType);
                return true;
            }

            // see if source or target types have a TypeConverter that converts between the two
            var toConverter = TypeDescriptor.GetConverter(initialType);
            if (toConverter != null && toConverter.CanConvertTo(targetType))
            {
                value = toConverter.ConvertTo(null, settings.Culture, initialValue, targetType);
                return true;
            }

            var fromConverter = TypeDescriptor.GetConverter(targetType);
            if (fromConverter != null && fromConverter.CanConvertFrom(initialType))
            {
                value = fromConverter.ConvertFrom(null, settings.Culture, initialValue);
                return true;
            }

            if (targetType.IsInterface || targetType.IsGenericTypeDefinition || targetType.IsAbstract)
            {
                value = null;
                return false;
            }

            value = null;
            return false;
        }

        public static bool IsNummeric(object value)
        {
            var type = GetTypeCode(value.GetType());
            switch (type)
            {
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        internal static BigInteger ToBigInteger(object value)
        {
            if (value is BigInteger bigInt)
            {
                return bigInt;
            }
            else if (value is string @string)
            {
                return BigInteger.Parse(@string, CultureInfo.InvariantCulture);
            }
            else if (value is float @float)
            {
                return new BigInteger(@float);
            }
            else if (value is double @double)
            {
                return new BigInteger(@double);
            }
            else if (value is decimal @decimal)
            {
                return new BigInteger(@decimal);
            }
            else if (value is int @int)
            {
                return new BigInteger(@int);
            }
            else if (value is long @long)
            {
                return new BigInteger(@long);
            }
            else if (value is uint @uint)
            {
                return new BigInteger(@uint);
            }
            else if (value is ulong @ulong)
            {
                return new BigInteger(@ulong);
            }
            else if (value is byte[] bytes)
            {
                return new BigInteger(bytes);
            }

            throw new InvalidCastException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Explicit casts required")]
        public static object FromBigInteger(BigInteger i, Type targetType)
        {
            return GetTypeCode(targetType) switch
            {
                PrimitiveTypeCode.Decimal => (decimal)i,
                PrimitiveTypeCode.Double => (double)i,
                PrimitiveTypeCode.Single => (float)i,
                PrimitiveTypeCode.UInt64Nullable => (ulong)i,
                PrimitiveTypeCode.Boolean => i != 0,
                _ => Convert.ChangeType((long)i, targetType, CultureInfo.InvariantCulture)
            };
        }
    }
}
