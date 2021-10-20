using Extensions.Dictionary.Converter;
using System;
using System.Collections.Generic;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class DateTimeConverterTests
    {
        private readonly ConverterSettings settings = new();
        private readonly DateTimeConverter converter = new();
        private readonly IDictionary<string, object> expectedMinimum = new Dictionary<string, object>
        {
            { nameof(DateTime.Ticks), 630823790450060000L },
            { nameof(DateTime.Kind), DateTimeKind.Utc },
        };
        private readonly IDictionary<string, object> expected = new Dictionary<string, object>
        {
            { nameof(DateTime.Year), 2000 },
            { nameof(DateTime.Month), 1 },
            { nameof(DateTime.Day), 2 },
            { nameof(DateTime.Hour), 3 },
            { nameof(DateTime.Minute), 4 },
            { nameof(DateTime.Second), 5 },
            { nameof(DateTime.Millisecond), 6 },
            { nameof(DateTime.Ticks), 630823790450060000L },
            { nameof(DateTime.Kind), DateTimeKind.Utc },
        };

        [Theory]
        [InlineData(EnumValueHandling.Default)]
        [InlineData(EnumValueHandling.UnderlyingValue)]
        public void ConvertToDateTime_Success(EnumValueHandling handling)
        {
            if (handling == EnumValueHandling.UnderlyingValue)
            {
                expected[nameof(DateTime.Kind)] = (int)expected[nameof(DateTime.Kind)];
            }

            settings.EnumHandling = handling;
            settings.DateHandling = DateValueHandling.Default;
            var date = new DateTime(2000, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            Assert.True(converter.CanConvert(date.GetType()));

            var dict = converter.ToDictionary(date, settings);
            Assert.Equal(expected, dict);

            dict.Remove(nameof(DateTime.Ticks));
            var result = converter.ToInstance(dict, settings);
            Assert.Equal(date, result);
        }

        [Theory]
        [InlineData(EnumValueHandling.Default)]
        [InlineData(EnumValueHandling.UnderlyingValue)]
        public void ConvertToMinimumDateTime_Success(EnumValueHandling handling)
        {
            if (handling == EnumValueHandling.UnderlyingValue)
            {
                expectedMinimum[nameof(DateTime.Kind)] = (int)expectedMinimum[nameof(DateTime.Kind)];
            }

            settings.EnumHandling = handling;
            settings.DateHandling = DateValueHandling.Minimum;
            var date = new DateTime(630823790450060000L, DateTimeKind.Utc);
            Assert.True(converter.CanConvert(date.GetType()));

            var dict = converter.ToDictionary(date, settings);
            Assert.Equal(expectedMinimum, dict);

            var result = converter.ToInstance(dict, settings);
            Assert.Equal(date, result);
        }
    }
}
