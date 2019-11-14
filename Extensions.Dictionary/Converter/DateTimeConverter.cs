using System;
using System.Collections.Generic;
using System.Globalization;

namespace Extensions.Dictionary.Converter
{
    internal sealed class DateTimeConverter : MemberConverter<DateTime>
    {
        public static readonly DateTimeConverter Default = new DateTimeConverter();

        public override IDictionary<string, object> ToDictionary(DateTime value, ConverterSettings settings)
        {
            return new Dictionary<string, object>(9)
            {
                { nameof(DateTime.Year), value.Year },
                { nameof(DateTime.Month), value.Month },
                { nameof(DateTime.Day), value.Day },
                { nameof(DateTime.Hour), value.Hour },
                { nameof(DateTime.Minute), value.Minute },
                { nameof(DateTime.Second), value.Second },
                { nameof(DateTime.Millisecond), value.Millisecond },
                { nameof(DateTime.Ticks), value.Ticks },
                { nameof(DateTime.Kind), value.Kind }
            };
        }

        public override DateTime ToInstance(IDictionary<string, object?> value, ConverterSettings settings)
        {
            if (!Enum.TryParse(value[nameof(DateTime.Kind)]?.ToString(), out DateTimeKind kind))
            {
                throw new InvalidOperationException();
            }

            if (value.TryGetValue(nameof(DateTime.Ticks), out object? t) && long.TryParse(t?.ToString(), out long ticks))
            {
                return new DateTime(ticks, kind);
            }

            var year = int.Parse(value[nameof(DateTime.Year)]?.ToString(), CultureInfo.InvariantCulture);
            var month = int.Parse(value[nameof(DateTime.Month)]?.ToString(), CultureInfo.InvariantCulture);
            var day = int.Parse(value[nameof(DateTime.Day)]?.ToString(), CultureInfo.InvariantCulture);
            var hour = int.Parse(value[nameof(DateTime.Hour)]?.ToString(), CultureInfo.InvariantCulture);
            var minute = int.Parse(value[nameof(DateTime.Minute)]?.ToString(), CultureInfo.InvariantCulture);
            var second = int.Parse(value[nameof(DateTime.Second)]?.ToString(), CultureInfo.InvariantCulture);
            var millisecond = int.Parse(value[nameof(DateTime.Millisecond)]?.ToString(), CultureInfo.InvariantCulture);
            return new DateTime(year, month, day, hour, minute, second, millisecond, kind);
        }
    }
}
