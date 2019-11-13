using System;
using System.Collections.Generic;

namespace Extensions.Dictionary
{
    public abstract class MemberConverter<T> : MemberConverter
    {
        public sealed override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public sealed override IDictionary<string, object> ToDictionary(object? value, ConverterSettings settings)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ToDictionary((T)value, settings);
        }

        public abstract IDictionary<string, object> ToDictionary(T value, ConverterSettings settings);

        public sealed override object? ToInstance(IDictionary<string, object?> value, Type type, ConverterSettings settings)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ToInstance(value, settings) as object;
        }

        public abstract T ToInstance(IDictionary<string, object?> value, ConverterSettings settings);
    }
}
