using Extensions.Dictionary.Converter;
using System;
using System.Collections.Generic;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class DateTimeConverterTests
    {
        private readonly ConverterSettings settings = new ConverterSettings();
        private readonly DateTimeConverter converter = new DateTimeConverter();
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
        [InlineData(true)]
        [InlineData(false)]
        public void ConvertToDateTime_Success(bool useTicks)
        {
            var date = useTicks
                ? new DateTime(630823790450060000L, DateTimeKind.Utc)
                : new DateTime(2000, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            Assert.True(converter.CanConvert(date.GetType()));

            var dict = converter.ToDictionary(date, settings);
            Assert.Equal(expected, dict);

            if (useTicks)
            {
                expected.Clear();
                expected.Add(nameof(DateTime.Ticks), 630823790450060000L);
            }

            var result = converter.ToInstance(dict, settings);
            Assert.Equal(date, result);
        }
    }
}
