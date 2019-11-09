using Extensions.Dictionary.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.Dictionary
{
    public static class DictionaryExtensions
    {
        private static readonly Type ObjectType = typeof(object);
        private static readonly Type GenericDictionaryType = typeof(IDictionary<,>);
        private static readonly Type[] AllowedEnumarableTypes = new[] { typeof(IList<>), typeof(IEnumerable<>), typeof(ICollection<>) };

        /// <summary>
        /// Converts a dictionary to the specific .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="serializerResolver">Optional serialzer resolver.</param>
        /// <param name="converters">Additional custom converter.</param>
        /// <returns>The converted object.</returns>
        public static T ToInstance<T>(this IDictionary<string, object?> dictionary, ISerializerResolver? serializerResolver = null, IEnumerable<JsonConverter>? converters = null)
                where T : new() => dictionary == null
                ? throw new ArgumentNullException(nameof(dictionary))
                : (T)ToInstanceInternal(dictionary, typeof(T), serializerResolver ?? DefaultResolver.Instance, GetSerializerOptions(converters));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object ToInstanceInternal(this IDictionary<string, object?> dictionary, Type type, ISerializerResolver serializerResolver, JsonSerializerOptions? options)
        {
            var instance = Activator.CreateInstance(type);
            var memberInfos = serializerResolver.GetMemberInfos(type);

            foreach (var element in dictionary)
            {
                var memberInfo = default(MemberInfo);
                for (int i = 0; i < memberInfos.Length; i++)
                {
                    if (memberInfos[i].GetName(serializerResolver) == element.Key || memberInfos[i].Name == element.Key)
                    {
                        memberInfo = memberInfos[i];
                        break;
                    }
                }

                if (memberInfo == null)
                {
                    // TODO: Think about a 'AllKeysMustMatch' solution
                    // throw new NotSupportedException($"Property or field '{type.Name}.{element.Key}' could not be found");
                    continue;
                }

                var value = element.Value;
                var memberInfoType = memberInfo.GetMemberType();
                if (element.Value != null && element.Value.GetType() != memberInfoType)
                {
                    var dict = (IDictionary<string, object?>?)element.Value ?? new Dictionary<string, object?>(0);
                    if (memberInfoType.IsGenericType)
                    {
                        var genericTypeDef = memberInfoType.GetGenericTypeDefinition();
                        var genericArgs = memberInfoType.GetGenericArguments();

                        if (genericTypeDef == GenericDictionaryType)
                        {   // Is Dictionary
                            if (genericArgs[1] != ObjectType)
                            {
                                value = dict.ConvertDictionary(memberInfoType);
                            }
                        }
                        else if (IsEnumerableType(genericTypeDef))
                        {   // IEnumerable
                            value = dict.Values.ConvertList(memberInfoType);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        value = ToInstanceInternal(dict, memberInfoType, serializerResolver, options);
                    }
                }

                memberInfo.SetValue(instance, value);
            }

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEnumerableType(Type type)
        {
            for (int i = 0; i < AllowedEnumarableTypes.Length; i++)
            {
                if (AllowedEnumarableTypes[i] == type)
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object ToInstanceFallback(object? value, Type type, JsonSerializerOptions? options = null) =>
            JsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(value), type, options);

        private static JsonSerializerOptions? GetSerializerOptions(IEnumerable<JsonConverter>? converters)
        {
            if (converters != null)
            {
                var options = new JsonSerializerOptions();
                foreach (var converter in converters)
                {
                    options.Converters.Add(converter);
                }

                return options;
            }

            return null;
        }

        private static object ConvertList(this ICollection<object?> items, Type type, bool convert = false)
        {
            var array = Array.CreateInstance(type.GenericTypeArguments[0], items.Count);
            var containedType = type.GenericTypeArguments[0];

            int i = 0;
            foreach (var item in items)
            {
                var value = convert
                    ? Convert.ChangeType(item, containedType, CultureInfo.InvariantCulture)
                    : item;
                array.SetValue(value, i);
                i++;
            }

            return array;
        }

        private static object ConvertDictionary(this IDictionary<string, object?> dict, Type type)
        {
            var dictToCast = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(new[] { type.GenericTypeArguments[0], type.GenericTypeArguments[1] }));
            foreach (var pair in dict)
            {
                dictToCast.Add(pair.Key, pair.Value);
            }

            return dictToCast;
        }
    }
}
