using Extensions.Dictionary.Internal;

namespace Extensions.Dictionary.Converter
{
    internal sealed class DateTimeOffsetConverter : MemberConverter<DateTimeOffset>
    {
        public static readonly DateTimeOffsetConverter Default = new();

        public override IDictionary<string, object> ToDictionary(DateTimeOffset value, ConverterSettings settings)
        {
            return settings.DateHandling switch
            {
                DateValueHandling.Default => new Dictionary<string, object>(9)
                {
                    { nameof(DateTimeOffset.Year), value.Year },
                    { nameof(DateTimeOffset.Month), value.Month },
                    { nameof(DateTimeOffset.Day), value.Day },
                    { nameof(DateTimeOffset.Hour), value.Hour },
                    { nameof(DateTimeOffset.Minute), value.Minute },
                    { nameof(DateTimeOffset.Second), value.Second },
                    { nameof(DateTimeOffset.Millisecond), value.Millisecond },
                    { nameof(DateTimeOffset.Ticks), value.Ticks },
                    { nameof(DateTimeOffset.Offset), TimespanConverter.Default.ToDictionary(value.Offset, settings) }
                },
                DateValueHandling.Minimum => new Dictionary<string, object>(2)
                {
                    { nameof(DateTimeOffset.Ticks), value.Ticks },
                    { nameof(DateTimeOffset.Offset), TimespanConverter.Default.ToDictionary(value.Offset, settings) }
                },
                _ => throw new NotSupportedException(),
            };
        }

        public override DateTimeOffset ToInstance(IDictionary<string, object?> value, ConverterSettings settings)
        {
            // Create Timespan
            var ts = default(TimeSpan);
            if (value.TryGetValue(nameof(DateTimeOffset.Offset), out object? offset) && offset is Dictionary<string, object?> offsetDict)
            {
                ts = TimespanConverter.Default.ToInstance(offsetDict, settings);
            }

            if (ts == default)
            {
                throw new InvalidCastException();
            }

            if (value.TryGetValue(nameof(DateTimeOffset.Ticks), out object? t) && long.TryParse(t?.ToString(), out long ticks))
            {
                return new DateTimeOffset(ticks, ts);
            }

            var year = value[nameof(DateTimeOffset.Year)].ConVal<int>(settings);
            var month = value[nameof(DateTimeOffset.Month)].ConVal<int>(settings);
            var day = value[nameof(DateTimeOffset.Day)].ConVal<int>(settings);
            var hour = value[nameof(DateTimeOffset.Hour)].ConVal<int>(settings);
            var minute = value[nameof(DateTimeOffset.Minute)].ConVal<int>(settings);
            var second = value[nameof(DateTimeOffset.Second)].ConVal<int>(settings);
            var millisecond = value[nameof(DateTimeOffset.Millisecond)].ConVal<int>(settings);
            return new DateTimeOffset(year, month, day, hour, minute, second, millisecond, ts);
        }
    }
}
