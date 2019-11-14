using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Extensions.Dictionary.Converter
{
    internal sealed class DefaultEnumerableConverter : CollectionMemberConverter<ICollection>
    {
        public static readonly DefaultEnumerableConverter Default = new DefaultEnumerableConverter();

        public override IDictionary<string, object> ToDictionary(ICollection value, ConverterSettings settings)
        {
            var dictionary = new Dictionary<string, object>(value.Count);
            if (value.GetType().GetGenericArguments()[0].IsSimpleType())
            {
                int i = 0;
                foreach (var element in value)
                {
                    dictionary[i++.ToString(CultureInfo.InvariantCulture)] = element;
                }
            }
            else
            {
                int i = 0;
                foreach (var element in value)
                {
                    dictionary[i++.ToString(CultureInfo.InvariantCulture)] = element.IsSimpleType()
                        ? element
                        : element.ToDictionaryInternal(settings);
                }
            }

            return dictionary;
        }

        public override ICollection ToInstance(IDictionary<string, object?> value, Type[] genericTypes, ConverterSettings settings)
        {
            var array = Array.CreateInstance(genericTypes[1], value.Count);
            int i = 0;
            foreach (var item in (Dictionary<string, object?>.ValueCollection)value.Values)
            {
                array.SetValue(item, i++);
            }

            return array;
        }
    }
}
