using System;
using System.Collections.Generic;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class CustomConverterTests
    {
        public struct CustomVector : IEquatable<CustomVector>
        {
            public float? X { get; set; }
            public float? Y { get; set; }
            public float? Z { get; set; }

            public bool Equals(CustomVector other) =>
                X == other.X && Y == other.Y && Z == other.Z;
        }

        public class FloatConverter : NativeConverter<float>
        {
            public override object ToDictionary(float value, ConverterSettings settings)
            {
                return value.ToString();
            }

            public override float ToInstance(object value, ConverterSettings settings)
            {
                return float.Parse(value.ToString());
            }
        }

        public class CustomVectorConverter : MemberConverter<CustomVector>
        {
            public override IDictionary<string, object> ToDictionary(CustomVector value, ConverterSettings settings)
            {
                var converter = settings.GetMatchingConverter(typeof(float?));
                if (converter != null)
                {
                    return new Dictionary<string, object>(3)
                    {
                        { nameof(CustomVector.X), converter.ToDictionary(999f, settings) },
                        { nameof(CustomVector.Y), converter.ToDictionary(999f, settings) },
                        { nameof(CustomVector.Z), converter.ToDictionary(null, settings) },
                    };
                }

                return new Dictionary<string, object>(3)
                {
                    { nameof(CustomVector.X), 999f },
                    { nameof(CustomVector.Y), 999f },
                    { nameof(CustomVector.Z), null },
                };
            }

            public override CustomVector ToInstance(IDictionary<string, object> value, ConverterSettings settings)
            {
                var converter = settings.GetMatchingConverter(typeof(float?));
                if (converter != null)
                {
                    return new CustomVector
                    {
                        X = (float?)converter.ToInstance(value[nameof(CustomVector.X)], typeof(float?), settings),
                        Y = (float?)converter.ToInstance(value[nameof(CustomVector.Y)], typeof(float?), settings),
                        Z = (float?)converter.ToInstance(value[nameof(CustomVector.Z)], typeof(float?), settings),
                    };
                }

                return new CustomVector
                {
                    X = (float?)value[nameof(CustomVector.X)],
                    Y = (float?)value[nameof(CustomVector.Y)],
                    Z = (float?)value[nameof(CustomVector.Z)],
                };
            }
        }

        private readonly ConverterSettings settings = new ConverterSettings();
        private readonly CustomVectorConverter customVectorConverter = new CustomVectorConverter();
        private readonly FloatConverter floatConverter = new FloatConverter();
        private readonly CustomVector vec1 = new CustomVector { X = 1f, Y = 2f, Z = null };
        private readonly CustomVector vec2 = new CustomVector { X = 999f, Y = 999f, Z = null };
        private readonly IDictionary<string, object> expected1 = new Dictionary<string, object>
        {
            { nameof(CustomVector.X), 1f },
            { nameof(CustomVector.Y), 2f },
            { nameof(CustomVector.Z), null },
        };
        private readonly IDictionary<string, object> expected2 = new Dictionary<string, object>
        {
            { nameof(CustomVector.X), 999f },
            { nameof(CustomVector.Y), 999f },
            { nameof(CustomVector.Z), null },
        };

        [Fact]
        public void ConvertToFloatBase_Success()
        {
            Assert.True(floatConverter.CanConvert(typeof(float)));

            var dict = floatConverter.ToDictionary((object)123f, settings);
            Assert.Equal("123", dict);

            var result = floatConverter.ToInstance(dict, typeof(float), settings);
            Assert.Equal(123f, result);
        }

        [Fact]
        public void ConvertToFloatWithNullValies_Exception()
        {
            settings.Converters.Add(floatConverter);
            Assert.True(floatConverter.CanConvert(typeof(float)));

            ArgumentNullException ex;
            ex = Assert.Throws<ArgumentNullException>(() => floatConverter.ToDictionary(null, settings));
            Assert.Equal("value", ex.ParamName);

            ex = Assert.Throws<ArgumentNullException>(() => floatConverter.ToInstance(null, typeof(float), settings));
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void ConvertToCustomVector3_Success()
        {
            Assert.True(customVectorConverter.CanConvert(vec1.GetType()));

            var dict = vec1.ToDictionary(settings);
            Assert.Equal(expected1, dict);

            var result = dict.ToInstance<CustomVector>(settings);
            Assert.Equal(vec1, result);

            settings.Converters.Add(customVectorConverter);

            dict = vec1.ToDictionary(settings);
            Assert.Equal(expected2, dict);

            result = dict.ToInstance<CustomVector>(settings);
            Assert.Equal(vec2, result);
        }

        [Fact]
        public void ConvertToCustomVector3Base_Success()
        {
            Assert.True(customVectorConverter.CanConvert(vec1.GetType()));

            var dict = customVectorConverter.ToDictionary((object)vec1, settings);
            Assert.Equal(expected2, dict);

            var result = customVectorConverter.ToInstance(dict, typeof(CustomVector), settings);
            Assert.Equal(vec2, result);
        }

        [Fact]
        public void ConvertToCustomVector3WithNullValues_Exception()
        {
            ArgumentNullException ex;
            ex = Assert.Throws<ArgumentNullException>(() => customVectorConverter.ToDictionary(null, settings));
            Assert.Equal("value", ex.ParamName);

            ex = Assert.Throws<ArgumentNullException>(() => customVectorConverter.ToInstance(null, typeof(CustomVector), settings));
            Assert.Equal("value", ex.ParamName);
        }
    }
}
