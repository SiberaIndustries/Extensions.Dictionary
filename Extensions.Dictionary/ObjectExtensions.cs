using Extensions.Dictionary.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Extensions.Dictionary
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to a dictionary recursively.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The object to convert.</param>
        /// <param name="serializerResolver">Optional serialzer resolver.</param>
        /// <returns>The converted dictionary.</returns>
        public static IDictionary<string, object?> ToDictionary<T>(this T instance, ISerializerResolver? serializerResolver = null)
            where T : new() => instance == null
            ? throw new ArgumentNullException(nameof(instance))
            : instance.ToDictionaryInternal(serializerResolver ?? DefaultResolver.Instance);

        internal static IDictionary<string, object?> ToDictionaryInternal(this object instance, ISerializerResolver serializerResolver)
        {
            if (instance is IDictionary dictionary)
            {
                return dictionary.ConvertFromDictionary(serializerResolver);
            }

            if (instance is ICollection entries)
            {
                return entries.ConvertFromEnumerable(serializerResolver);
            }

            var members = serializerResolver.GetMemberInfos(instance.GetType());
            var resultDictionary = new Dictionary<string, object?>(members.Length);
            foreach (var member in members)
            {
                var value = member.GetValue(instance, serializerResolver);
                resultDictionary[member.GetName(serializerResolver)] = value == null || value.IsSimpleType()
                    ? value
                    : value.ToDictionaryInternal(serializerResolver);
            }

            return resultDictionary;
        }

        internal static IDictionary<string, object?> ConvertFromDictionary(this IDictionary dictionary, ISerializerResolver serializerResolver)
        {
            if (dictionary is IDictionary<string, object?> dict)
            {
                return dict;
            }

            var resultDictionary = new Dictionary<string, object?>(dictionary.Count);
            if (dictionary.GetType().GetGenericArguments()[1].IsSimpleType())
            {
                foreach (var key in dictionary.Keys)
                {
                    resultDictionary[key.ToString()] = dictionary[key];
                }
            }
            else
            {
                foreach (var key in dictionary.Keys)
                {
                    resultDictionary[key.ToString()] = dictionary[key].IsSimpleType()
                        ? dictionary[key]
                        : dictionary[key].ToDictionaryInternal(serializerResolver);
                }
            }

            return resultDictionary;
        }

        internal static IDictionary<string, object?> ConvertFromEnumerable(this ICollection entries, ISerializerResolver serializerResolver)
        {
            var resultDictionary = new Dictionary<string, object?>(entries.Count);
            if (entries.GetType().GetGenericArguments()[0].IsSimpleType())
            {
                int i = 0;
                foreach (var value in entries)
                {
                    resultDictionary[i++.ToString(CultureInfo.InvariantCulture)] = value;
                }
            }
            else
            {
                int i = 0;
                foreach (var value in entries)
                {
                    resultDictionary[i++.ToString(CultureInfo.InvariantCulture)] = value.IsSimpleType()
                        ? value
                        : value.ToDictionaryInternal(serializerResolver);
                }
            }

            return resultDictionary;
        }

        internal static bool IsSimpleType(this object? instance) =>
            instance == null || instance.GetType().IsSimpleType();
    }
}
