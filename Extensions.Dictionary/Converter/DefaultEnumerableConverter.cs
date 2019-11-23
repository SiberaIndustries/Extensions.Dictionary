using Extensions.Dictionary.Internal;
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
            var valueType = value.GetType();
            var itemType = valueType.IsArray ? valueType : valueType.GetGenericArguments()[0];
            var dictionary = new Dictionary<string, object>(value.Count);
            if (itemType.IsSimpleType())
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
                    dictionary[i++.ToString(CultureInfo.InvariantCulture)] = element.GetType().IsSimpleType()
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
