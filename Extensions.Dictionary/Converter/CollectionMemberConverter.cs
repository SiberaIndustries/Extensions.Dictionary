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

        public sealed override object Convert(object? value, ConverterSettings settings)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Convert((T)value, settings);
        }

        public abstract IDictionary<string, object> Convert(T value, ConverterSettings settings);

        public sealed override object? ConvertBack(object value, Type type, ConverterSettings settings)
        {
            GenericTypes[1] = type;
            return ConvertBack((IDictionary<string, object?>)value, GenericTypes, settings) as object;
        }

        public abstract T ConvertBack(IDictionary<string, object?> value, Type[] genericTypes, ConverterSettings settings);
    }
}
