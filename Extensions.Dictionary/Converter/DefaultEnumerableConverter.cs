using Extensions.Dictionary.Internal;
using System.Collections;
using System.Globalization;

namespace Extensions.Dictionary.Converter
{
    internal sealed class DefaultEnumerableConverter : CollectionMemberConverter<ICollection>
    {
        public static readonly DefaultEnumerableConverter Default = new();

        public override IDictionary<string, object> Convert(ICollection value, ConverterSettings settings)
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

        public override ICollection ConvertBack(IDictionary<string, object?> value, Type[] genericTypes, ConverterSettings settings)
        {
            var elementType = genericTypes[1];
            if (elementType.IsGenericType)
            {
                elementType = elementType.GetGenericArguments()[0];
            }
            else if (elementType.IsArray)
            {
                elementType = elementType.GetElementType();
            }

            var array = Array.CreateInstance(elementType!, value.Count);
            int i = 0;
            foreach (var item in (Dictionary<string, object?>.ValueCollection)value.Values)
            {
                array.SetValue(item, i++);
            }

            return array;
        }
    }
}
