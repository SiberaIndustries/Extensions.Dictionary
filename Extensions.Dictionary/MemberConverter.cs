using System;

namespace Extensions.Dictionary
{
    public abstract class MemberConverter
    {
        public abstract bool CanConvert(Type objectType);

        public abstract object Convert(object? value, ConverterSettings settings);

        public abstract object? ConvertBack(object value, Type type, ConverterSettings settings);
    }
}
