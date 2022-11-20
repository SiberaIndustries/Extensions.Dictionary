using Extensions.Dictionary.Converter;
using System;
using System.Collections;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class DateTimeOffsetConverterTests
    {
        private readonly ConverterSettings settings = new();
        private readonly DateTimeOffsetConverter converter = new();
        private readonly IDictionary<string, object> expectedMinimum = new Dictionary<string, object>
        {
            { nameof(DateTimeOffset.Ticks), 630823790450060000L },
            { nameof(DateTimeOffset.Offset), new Dictionary<string, object>
                {
                    { nameof(TimeSpan.Ticks), 4200000000L },
                } },
        };
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

        [Fact]
        public void ConvertToDateTimeOffset_Success()
        {
            settings.DateHandling = DateValueHandling.Default;
            var date = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromMinutes(7));
            Assert.True(converter.CanConvert(date.GetType()));

            var dict = converter.ToDictionary(date, settings);
            Assert.Equal(expected, dict);

            dict.Remove(nameof(DateTime.Ticks));
            ((IDictionary)dict[nameof(DateTimeOffset.Offset)]).Remove(nameof(TimeSpan.Ticks));
            var result = converter.ToInstance(dict, settings);
            Assert.Equal(date, result);
        }

        [Fact]
        public void ConvertToMinimumDateTimeOffset_Success()
        {
            settings.DateHandling = DateValueHandling.Minimum;
            var date = new DateTimeOffset(630823790450060000L, TimeSpan.FromMinutes(7));
            Assert.True(converter.CanConvert(date.GetType()));

            var dict = converter.ToDictionary(date, settings);
            Assert.Equal(expectedMinimum, dict);

            var result = converter.ToInstance(dict, settings);
            Assert.Equal(date, result);
        }
    }
}
