using Extensions.Dictionary.Internal;

namespace Extensions.Dictionary.Converter
{
    internal sealed class TimespanConverter : MemberConverter<TimeSpan>
    {
        public static readonly TimespanConverter Default = new();

        public override IDictionary<string, object> ToDictionary(TimeSpan value, ConverterSettings settings)
        {
            return settings.DateHandling switch
            {
                DateValueHandling.Default => new Dictionary<string, object>(5)
                {
                    { nameof(TimeSpan.Days), value.Days },
                    { nameof(TimeSpan.Hours), value.Hours },
                    { nameof(TimeSpan.Minutes), value.Minutes },
                    { nameof(TimeSpan.Seconds), value.Seconds },
                    { nameof(TimeSpan.Milliseconds), value.Milliseconds },
                    { nameof(TimeSpan.Ticks), value.Ticks },
                },
                DateValueHandling.Minimum => new Dictionary<string, object>(1)
                {
                    { nameof(TimeSpan.Ticks), value.Ticks },
                },
                _ => throw new NotSupportedException()
            };
        }

        public override TimeSpan ToInstance(IDictionary<string, object?> value, ConverterSettings settings)
        {
            if (value.TryGetValue(nameof(TimeSpan.Ticks), out object? tsTicksObj) && long.TryParse(tsTicksObj?.ToString(), out long tsTicks))
            {
                return new TimeSpan(tsTicks);
            }

            var tsDays = value[nameof(TimeSpan.Days)].ConVal<int>(settings);
            var tsHours = value[nameof(TimeSpan.Hours)].ConVal<int>(settings);
            var tsMinutes = value[nameof(TimeSpan.Minutes)].ConVal<int>(settings);
            var tsSeconds = value[nameof(TimeSpan.Seconds)].ConVal<int>(settings);
            var tsMilliseconds = value[nameof(TimeSpan.Milliseconds)].ConVal<int>(settings);
            return new TimeSpan(tsDays, tsHours, tsMinutes, tsSeconds, tsMilliseconds);
        }
    }
}
