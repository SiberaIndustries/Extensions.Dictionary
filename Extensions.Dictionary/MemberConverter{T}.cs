namespace Extensions.Dictionary
{
    public abstract class MemberConverter<T> : MemberConverter
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

            return ToDictionary((T)value, settings);
        }

        public abstract IDictionary<string, object> ToDictionary(T value, ConverterSettings settings);

        public sealed override object? ConvertBack(object value, Type type, ConverterSettings settings)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ToInstance((IDictionary<string, object?>)value, settings) as object;
        }

        public abstract T ToInstance(IDictionary<string, object?> value, ConverterSettings settings);
    }
}
