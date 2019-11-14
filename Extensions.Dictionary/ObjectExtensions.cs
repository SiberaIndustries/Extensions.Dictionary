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
        /// <param name="settings">Optional converter settings.</param>
        /// <returns>The converted dictionary.</returns>
        public static IDictionary<string, object?> ToDictionary<T>(this T instance, ConverterSettings? settings = null)
            where T : new() => instance == null
            ? throw new ArgumentNullException(nameof(instance))
            : instance.ToDictionaryInternal(settings ?? new ConverterSettings());

        internal static IDictionary<string, object?> ToDictionaryInternal(this object instance, ConverterSettings settings)
        {
            if (instance is IDictionary dictionary)
            {
                return dictionary.ConvertFromDictionary(settings);
            }

            if (instance is ICollection entries)
            {
                return entries.ConvertFromEnumerable(settings);
            }

            var resolver = settings.ResolverInternal;
            var members = resolver.GetMemberInfos(instance.GetType());
            var resultDictionary = new Dictionary<string, object?>(members.Length);
            foreach (var member in members)
            {
                var key = member.GetName(resolver);
                var value = member.GetValue(instance, resolver);
                if (value == null || value.IsSimpleType())
                {
                    resultDictionary[key] = value;
                    continue;
                }

                var converter = settings.GetMatchingConverter(value.GetType());
                if (converter != null)
                {
                    resultDictionary[key] = converter.ToDictionary(value, settings);
                    continue;
                }

                resultDictionary[key] = value.ToDictionaryInternal(settings);
            }

            return resultDictionary;
        }

        internal static IDictionary<string, object?> ConvertFromDictionary(this IDictionary dictionary, ConverterSettings settings)
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
                        : dictionary[key].ToDictionaryInternal(settings);
                }
            }

            return resultDictionary;
        }

        internal static IDictionary<string, object?> ConvertFromEnumerable(this ICollection entries, ConverterSettings settings)
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
                        : value.ToDictionaryInternal(settings);
                }
            }

            return resultDictionary;
        }

        internal static bool IsSimpleType(this object? instance) =>
            instance == null || instance.GetType().IsSimpleType();
    }
}
