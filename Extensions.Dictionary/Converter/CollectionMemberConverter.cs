using System;
using System.Collections.Generic;

namespace Extensions.Dictionary.Converter
{
    public abstract class CollectionMemberConverter<T> : MemberConverter
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
            return ToInstance(value, new[] { typeof(string), type }, settings) as object;
        }

        public abstract T ToInstance(IDictionary<string, object?> value, Type[] genericTypes, ConverterSettings settings);
    }
}
