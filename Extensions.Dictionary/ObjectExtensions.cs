using Extensions.Dictionary.Internal;
using System;
using System.Collections.Generic;

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
            var instanceType = instance.GetType();
            var converter = settings.GetMatchingConverter(instanceType);
            if (converter != null)
            {
                return (IDictionary<string, object?>)converter.ToDictionary(instance, settings);
            }

            var resolver = settings.ResolverInternal;
            var members = resolver.GetMemberInfos(instanceType);
            var resultDictionary = new Dictionary<string, object?>(members.Length);
            foreach (var member in members)
            {
                var key = member.GetName(resolver);
                var value = member.GetValue(instance, resolver);
                if (value == null || value.GetType().IsSimpleType())
                {
                    resultDictionary[key] = value;
                    continue;
                }

                converter = settings.GetMatchingConverter(value.GetType());
                if (converter != null)
                {
                    resultDictionary[key] = converter.ToDictionary(value, settings);
                    continue;
                }

                resultDictionary[key] = value.ToDictionaryInternal(settings);
            }

            return resultDictionary;
        }
    }
}
