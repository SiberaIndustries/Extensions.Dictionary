using System;

namespace Extensions.Dictionary
{
    public abstract class NativeConverter<T> : MemberConverter
    {
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

        public abstract object ToDictionary(T value, ConverterSettings settings);

        public sealed override object? ToInstance(object value, Type type, ConverterSettings settings)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ToInstance(value, settings) as object;
        }

        public abstract T ToInstance(object value, ConverterSettings settings);
    }
}
