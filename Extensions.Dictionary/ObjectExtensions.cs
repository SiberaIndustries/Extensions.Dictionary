using Extensions.Dictionary.Resolver;
using System.Collections;
using System.Collections.Generic;

namespace Extensions.Dictionary
{
    public static class ObjectExtensions
    {
        internal static bool IsSimpleType(this object? instance) =>
            instance == null || instance.GetType().IsSimpleType();

        /// <summary>
        /// Converts an object to a dictionary recursively.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The object to convert.</param>
        /// <param name="serializerResolver">Optional serialzer resolver.</param>
        /// <returns>The converted dictionary.</returns>
        public static IDictionary<string, object?> ToDictionary<T>(this T instance, ISerializerResolver? serializerResolver = null)
            where T : new() =>
            instance.ToDictionaryInternal(serializerResolver ?? DefaultResolver.Instance);

        internal static IDictionary<string, object?> ToDictionaryInternal<T>(this T instance, in ISerializerResolver serializerResolver)
           where T : new()
        {
            var resultDictionary = new Dictionary<string, object?>();

            // Dictionary
            if (instance is IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys)
                {
                    resultDictionary[key.ToString()] = dictionary[key].IsSimpleType()
                        ? dictionary[key]
                        : dictionary[key].ToDictionaryInternal(serializerResolver);
                }

                return resultDictionary;
            }

            // Array
            if (instance is IEnumerable entries)
            {
                int i = 0;
                foreach (var value in entries)
                {
                    resultDictionary[string.Empty + i++] = value.IsSimpleType()
                        ? value
                        : value.ToDictionaryInternal(serializerResolver);
                }

                return resultDictionary;
            }

            // Everything else
            var members = serializerResolver.GetMemberInfos(instance?.GetType());
            foreach (var member in members)
            {
                var value = member.GetValue(instance, serializerResolver);
                resultDictionary[member.GetName(serializerResolver)] = value.IsSimpleType()
                    ? value
                    : value.ToDictionaryInternal(serializerResolver);
            }

            return resultDictionary;
        }
    }
}
