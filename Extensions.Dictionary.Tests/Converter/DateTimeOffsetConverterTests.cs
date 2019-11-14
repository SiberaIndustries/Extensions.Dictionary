using Extensions.Dictionary.Converter;
using System;
using System.Collections.Generic;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class DateTimeOffsetConverterTests
    {
        private readonly ConverterSettings settings = new ConverterSettings();
        private readonly DateTimeOffsetConverter converter = new DateTimeOffsetConverter();
        private readonly IDictionary<string, object> expected = new Dictionary<string, object>
        {
            { nameof(DateTimeOffset.Year), 2000 },
            { nameof(DateTimeOffset.Month), 1 },
            { nameof(DateTimeOffset.Day), 2 },
            { nameof(DateTimeOffset.Hour), 3 },
            { nameof(DateTimeOffset.Minute), 4 },
            { nameof(DateTimeOffset.Second), 5 },
            { nameof(DateTimeOffset.Millisecond), 6 },
            { nameof(DateTimeOffset.Ticks), 630823790450060000L },
            { nameof(DateTimeOffset.Offset), new Dictionary<string, object>
                {
                    { nameof(TimeSpan.Days), 0 },
                    { nameof(TimeSpan.Hours), 0 },
                    { nameof(TimeSpan.Minutes), 7 },
                    { nameof(TimeSpan.Seconds), 0 },
                    { nameof(TimeSpan.Milliseconds), 0 },
                    { nameof(TimeSpan.Ticks), 4200000000L },
                } },
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ConvertToDateTimeOffset_Success(bool useTicks)
        {
            var date = useTicks
                ? new DateTimeOffset(630823790450060000L, TimeSpan.FromMinutes(7))
                : new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromMinutes(7));
            Assert.True(converter.CanConvert(date.GetType()));

            var dict = converter.ToDictionary(date, settings);
            Assert.Equal(expected, dict);

            if (!useTicks)
            {
                dict.Remove(nameof(DateTime.Ticks));
            }

            var result = converter.ToInstance(dict, settings);
            Assert.Equal(date, result);
        }
    }
}
