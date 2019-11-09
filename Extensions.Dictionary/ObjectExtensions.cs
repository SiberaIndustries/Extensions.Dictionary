using Extensions.Dictionary.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Extensions.Dictionary
{
    public static class ObjectExtensions
    {
        private const BindingFlags PublicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
        private const string Key = "Key";
        private const string Value = "Value";

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
            var resultDictionary = new Dictionary<string, object?>();
            var type = instance.GetType();
            if (type.IsGenericType)
            {
                var gernTypeDef = type.GetGenericTypeDefinition();
                if (gernTypeDef == typeof(Dictionary<,>))
                {   // Dictionary
                    if (type.GetGenericArguments()[1].IsSimpleType())
                    {
                        foreach (var item in (IEnumerable)instance)
                        {
                            var itemType = item.GetType();
                            var value = itemType.GetProperty(Value, PublicInstanceFlags).GetValue(item);
                            resultDictionary[itemType.GetProperty(Key, PublicInstanceFlags).GetValue(item).ToString()] = value;
                        }
                    }
                    else
                    {
                        foreach (var item in (IEnumerable)instance)
                        {
                            var itemType = item.GetType();
                            var value = itemType.GetProperty(Value, PublicInstanceFlags).GetValue(item);
                            resultDictionary[itemType.GetProperty(Key, PublicInstanceFlags).GetValue(item).ToString()] = value.IsSimpleType()
                                ? value
                                : value.ToDictionaryInternal(serializerResolver);
                        }
                    }
                }
                else
                {   // Array
                    if (type.GetGenericArguments()[0].IsSimpleType())
                    {
                        var i = 0;
                        foreach (var item in (IEnumerable)instance)
                        {
                            resultDictionary[i++.ToString(CultureInfo.InvariantCulture)] = item;
                        }
                    }
                    else
                    {
                        var i = 0;
                        foreach (var item in (IEnumerable)instance)
                        {
                            resultDictionary[i++.ToString(CultureInfo.InvariantCulture)] = item.IsSimpleType()
                                ? item
                                : item.ToDictionaryInternal(serializerResolver);
                        }
                    }
                }

                return resultDictionary;
            }

            // Everything else
            var members = serializerResolver.GetMemberInfos(type);
            foreach (var member in members)
            {
                var value = member.GetValue(instance, serializerResolver);
                resultDictionary[member.GetName(serializerResolver)] = value == null || value.IsSimpleType()
                    ? value
                    : value.ToDictionaryInternal(serializerResolver);
            }

            return resultDictionary;
        }

        internal static bool IsSimpleType(this object? instance) =>
            instance == null || instance.GetType().IsSimpleType();
    }
}
