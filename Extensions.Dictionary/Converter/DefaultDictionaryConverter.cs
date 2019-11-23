using Extensions.Dictionary.Internal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Extensions.Dictionary.Converter
{
    internal sealed class DefaultDictionaryConverter : CollectionMemberConverter<IDictionary>
    {
        public static readonly DefaultDictionaryConverter Default = new DefaultDictionaryConverter();

        public override IDictionary<string, object> ToDictionary(IDictionary value, ConverterSettings settings)
        {
            if (value is IDictionary<string, object> dictionary)
            {
                return dictionary;
            }

            dictionary = new Dictionary<string, object>(value.Count);
            if (value.GetType().GetGenericArguments()[1].IsSimpleType())
            {
                foreach (var key in value.Keys)
                {
                    dictionary[key.ToString()] = value[key];
                }
            }
            else
            {
                foreach (var key in value.Keys)
                {
                    var keyString = key.ToString();
                    var keyValue = value[key];
                    dictionary[keyString] = keyValue.GetType().IsSimpleType()
                        ? keyValue
                        : ToDictionary(dictionary, settings);
                }
            }

            return dictionary;
        }

        public override IDictionary ToInstance(IDictionary<string, object?> value, Type[] genericTypes, ConverterSettings settings)
        {
            if (genericTypes[1] != typeof(object))
            {
                GenericTypes[0] = genericTypes[0];
                GenericTypes[1] = genericTypes[1];
                var dictToCast = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(GenericTypes));
                foreach (var pair in (Dictionary<string, object?>)value)
                {
                    dictToCast.Add(pair.Key, pair.Value);
                }

                return dictToCast;
            }

            return (IDictionary)value;
        }
    }
}
