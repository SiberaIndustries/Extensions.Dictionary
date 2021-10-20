using Extensions.Dictionary.Internal;
using System;
using System.Collections.Generic;

namespace Extensions.Dictionary.Converter
{
    internal sealed class DateTimeConverter : MemberConverter<DateTime>
    {
        public static readonly DateTimeConverter Default = new();

        public override IDictionary<string, object> ToDictionary(DateTime value, ConverterSettings settings)
        {
            var dictionary = settings.DateHandling switch
            {
                DateValueHandling.Default => new Dictionary<string, object>(9)
                {
                    { nameof(DateTime.Year), value.Year },
                    { nameof(DateTime.Month), value.Month },
                    { nameof(DateTime.Day), value.Day },
                    { nameof(DateTime.Hour), value.Hour },
                    { nameof(DateTime.Minute), value.Minute },
                    { nameof(DateTime.Second), value.Second },
                    { nameof(DateTime.Millisecond), value.Millisecond },
                    { nameof(DateTime.Ticks), value.Ticks },
                },
                DateValueHandling.Minimum => new Dictionary<string, object>(2)
                {
                    { nameof(DateTime.Ticks), value.Ticks },
                },
                _ => throw new NotSupportedException(),
            };

            switch (settings.EnumHandling)
            {
                case EnumValueHandling.Default:
                    dictionary[nameof(DateTime.Kind)] = value.Kind;
                    return dictionary;
                case EnumValueHandling.UnderlyingValue:
                    dictionary[nameof(DateTime.Kind)] = (int)value.Kind;
                    return dictionary;
                default:
                    throw new NotSupportedException();
            }
        }

        public override DateTime ToInstance(IDictionary<string, object?> value, ConverterSettings settings)
        {
            if (!Enum.TryParse(value[nameof(DateTime.Kind)]?.ToString(), out DateTimeKind kind))
            {
                throw new InvalidOperationException();
            }

            if (value.TryGetValue(nameof(DateTime.Ticks), out object? ticksObj) && long.TryParse(ticksObj?.ToString(), out long ticks))
            {
                return new DateTime(ticks, kind);
            }

            var year = value[nameof(DateTime.Year)].ConVal<int>(settings);
            var month = value[nameof(DateTime.Month)].ConVal<int>(settings);
            var day = value[nameof(DateTime.Day)].ConVal<int>(settings);
            var hour = value[nameof(DateTime.Hour)].ConVal<int>(settings);
            var minute = value[nameof(DateTime.Minute)].ConVal<int>(settings);
            var second = value[nameof(DateTime.Second)].ConVal<int>(settings);
            var millisecond = value[nameof(DateTime.Millisecond)].ConVal<int>(settings);
            return new DateTime(year, month, day, hour, minute, second, millisecond, kind);
        }
    }
}
