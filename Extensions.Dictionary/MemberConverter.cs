using System;
using System.Collections.Generic;

namespace Extensions.Dictionary
{
    public abstract class MemberConverter
    {
        public abstract bool CanConvert(Type objectType);

        public abstract IDictionary<string, object> ToDictionary(object? value, ConverterSettings settings);

        public abstract object? ToInstance(IDictionary<string, object?> value, Type type, ConverterSettings settings);
    }
}
