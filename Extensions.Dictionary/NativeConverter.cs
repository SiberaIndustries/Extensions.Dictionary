using System;

namespace Extensions.Dictionary
{
    public abstract class NativeConverter<T> : MemberConverter
    {
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

        public abstract object Convert(T value, ConverterSettings settings);

        public sealed override object? ConvertBack(object value, Type type, ConverterSettings settings)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ConvertBack(value, settings) as object;
        }

        public abstract T ConvertBack(object value, ConverterSettings settings);
    }
}
