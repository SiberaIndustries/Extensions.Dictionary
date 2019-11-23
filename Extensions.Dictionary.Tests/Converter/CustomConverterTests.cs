using System.Collections.Generic;
using System.Numerics;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class CustomConverterTests
    {
        public class Vector3Converter : MemberConverter<Vector3>
        {
            public override IDictionary<string, object> ToDictionary(Vector3 value, ConverterSettings settings)
            {
                return new Dictionary<string, object>(3)
            {
                { nameof(Vector3.X), 999f },
                { nameof(Vector3.Y), 999f },
                { nameof(Vector3.Z), 999f },
            };
            }

            public override Vector3 ToInstance(IDictionary<string, object> value, ConverterSettings settings)
            {
                return new Vector3
                {
                    X = float.Parse(value[nameof(Vector3.X)]?.ToString(), settings.Culture),
                    Y = float.Parse(value[nameof(Vector3.Y)]?.ToString(), settings.Culture),
                    Z = float.Parse(value[nameof(Vector3.Z)]?.ToString(), settings.Culture),
                };
            }
        }

        private readonly ConverterSettings settings = new ConverterSettings();
        private readonly Vector3Converter converter = new Vector3Converter();
        private readonly IDictionary<string, object> expected1 = new Dictionary<string, object>
        {
            { nameof(Vector3.X), 1f },
            { nameof(Vector3.Y), 2f },
            { nameof(Vector3.Z), 3f },
        };
        private readonly IDictionary<string, object> expected2 = new Dictionary<string, object>
        {
            { nameof(Vector3.X), 999f },
            { nameof(Vector3.Y), 999f },
            { nameof(Vector3.Z), 999f },
        };

        [Fact]
        public void ConvertToCustomVector3_Success()
        {
            var vec = new Vector3(1f, 2f, 3f);
            Assert.True(converter.CanConvert(vec.GetType()));

            var dict = vec.ToDictionary(settings);
            Assert.Equal(expected1, dict);

            var result = dict.ToInstance<Vector3>(settings);
            Assert.Equal(vec, result);

            settings.Converters.Add(converter);

            dict = vec.ToDictionary(settings);
            Assert.Equal(expected2, dict);

            result = dict.ToInstance<Vector3>(settings);
            Assert.NotEqual(vec, result);
        }
    }
}
