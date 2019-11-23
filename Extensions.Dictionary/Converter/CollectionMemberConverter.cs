using System;
using System.Collections.Generic;

namespace Extensions.Dictionary.Converter
{
    internal abstract class CollectionMemberConverter<T> : MemberConverter
    {
        protected readonly Type[] GenericTypes = { typeof(string), typeof(object) };

        public sealed override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public sealed override object ToDictionary(object? value, ConverterSettings settings)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ToDictionary((T)value, settings);
        }

        public abstract IDictionary<string, object> ToDictionary(T value, ConverterSettings settings);

        public sealed override object? ToInstance(object value, Type type, ConverterSettings settings)
        {
            GenericTypes[1] = type;
            return ToInstance((IDictionary<string, object?>)value, GenericTypes, settings) as object;
        }

        public abstract T ToInstance(IDictionary<string, object?> value, Type[] genericTypes, ConverterSettings settings);
    }
}
