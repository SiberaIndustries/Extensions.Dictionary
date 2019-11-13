using System;
using System.Collections.Generic;
using System.Globalization;

namespace Extensions.Dictionary.Converter
{
    internal sealed class DateTimeOffsetConverter : MemberConverter<DateTimeOffset>
    {
        public static readonly DateTimeOffsetConverter Default = new DateTimeOffsetConverter();

        public override IDictionary<string, object> ToDictionary(DateTimeOffset value, ConverterSettings settings)
        {
            var tsDict = new Dictionary<string, object>(5)
            {
                { nameof(TimeSpan.Days), value.Offset.Days },
                { nameof(TimeSpan.Hours), value.Offset.Hours },
                { nameof(TimeSpan.Minutes), value.Offset.Minutes },
                { nameof(TimeSpan.Seconds), value.Offset.Seconds },
                { nameof(TimeSpan.Milliseconds), value.Offset.Milliseconds },
            };

            return new Dictionary<string, object>(9)
            {
                { nameof(DateTimeOffset.Year), value.Year },
                { nameof(DateTimeOffset.Month), value.Month },
                { nameof(DateTimeOffset.Day), value.Day },
                { nameof(DateTimeOffset.Hour), value.Hour },
                { nameof(DateTimeOffset.Minute), value.Minute },
                { nameof(DateTimeOffset.Second), value.Second },
                { nameof(DateTimeOffset.Millisecond), value.Millisecond },
                { nameof(DateTimeOffset.Ticks), value.Ticks },
                { nameof(DateTimeOffset.Offset), tsDict }
            };
        }

        public override DateTimeOffset ToInstance(IDictionary<string, object?> value, ConverterSettings settings)
        {
            // Create Timespan
            var ts = default(TimeSpan);
            if (value.TryGetValue(nameof(DateTimeOffset.Offset), out object? offset) && offset is Dictionary<string, object> offsetDict)
            {
                if (offsetDict.TryGetValue(nameof(TimeSpan.Ticks), out object? tsTicksObj) && long.TryParse(tsTicksObj?.ToString(), out long tsTicks))
                {
                    ts = new TimeSpan(tsTicks);
                }
                else
                {
                    var tsDays = int.Parse(offsetDict[nameof(TimeSpan.Days)]?.ToString(), CultureInfo.InvariantCulture);
                    var tsHours = int.Parse(offsetDict[nameof(TimeSpan.Hours)]?.ToString(), CultureInfo.InvariantCulture);
                    var tsMinutes = int.Parse(offsetDict[nameof(TimeSpan.Minutes)]?.ToString(), CultureInfo.InvariantCulture);
                    var tsSeconds = int.Parse(offsetDict[nameof(TimeSpan.Seconds)]?.ToString(), CultureInfo.InvariantCulture);
                    var tsMilliseconds = int.Parse(offsetDict[nameof(TimeSpan.Milliseconds)]?.ToString(), CultureInfo.InvariantCulture);
                    ts = new TimeSpan(tsDays, tsHours, tsMinutes, tsSeconds, tsMilliseconds);
                }
            }

            if (ts == default)
            {
                throw new InvalidCastException();
            }

            if (value.TryGetValue(nameof(DateTimeOffset.Ticks), out object? t) && long.TryParse(t?.ToString(), out long ticks))
            {
                return new DateTimeOffset(ticks, ts);
            }

            var year = int.Parse(value[nameof(DateTimeOffset.Year)]?.ToString(), CultureInfo.InvariantCulture);
            var month = int.Parse(value[nameof(DateTimeOffset.Month)]?.ToString(), CultureInfo.InvariantCulture);
            var day = int.Parse(value[nameof(DateTimeOffset.Day)]?.ToString(), CultureInfo.InvariantCulture);
            var hour = int.Parse(value[nameof(DateTimeOffset.Hour)]?.ToString(), CultureInfo.InvariantCulture);
            var minute = int.Parse(value[nameof(DateTimeOffset.Minute)]?.ToString(), CultureInfo.InvariantCulture);
            var second = int.Parse(value[nameof(DateTimeOffset.Second)]?.ToString(), CultureInfo.InvariantCulture);
            var millisecond = int.Parse(value[nameof(DateTimeOffset.Millisecond)]?.ToString(), CultureInfo.InvariantCulture);
            return new DateTimeOffset(year, month, day, hour, minute, second, millisecond, ts);
        }
    }
}
