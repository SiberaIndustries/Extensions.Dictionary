using System;

namespace Extensions.Dictionary
{
    public abstract class MemberConverter
    {
        public abstract bool CanConvert(Type objectType);

        public abstract object ToDictionary(object? value, ConverterSettings settings);

        public abstract object? ToInstance(object value, Type type, ConverterSettings settings);
    }
}
