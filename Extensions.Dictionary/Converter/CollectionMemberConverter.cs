namespace Extensions.Dictionary.Converter
{
    internal abstract class CollectionMemberConverter<T> : MemberConverter
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

        public abstract IDictionary<string, object> Convert(T value, ConverterSettings settings);

        public sealed override object? ConvertBack(object value, Type type, ConverterSettings settings)
        {
            return ConvertBack((IDictionary<string, object?>)value, new[] { typeof(string), type }, settings);
        }

        public abstract T ConvertBack(IDictionary<string, object?> value, Type[] genericTypes, ConverterSettings settings);
    }
}
