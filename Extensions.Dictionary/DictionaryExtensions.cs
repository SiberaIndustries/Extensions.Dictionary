using Extensions.Dictionary.Converter;
using Extensions.Dictionary.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Extensions.Dictionary
{
    public static class DictionaryExtensions
    {
        private static readonly Type ObjectType = typeof(object);
        private static readonly Type GenericDictionaryType = typeof(Dictionary<,>);
        private static readonly Type GenericDictionaryInterfaceType = typeof(IDictionary<,>);
        private static readonly Type EnumerableType = typeof(IEnumerable);

        /// <summary>
        /// Converts a dictionary to the specific .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="settings">Optional converter settings.</param>
        /// <returns>The converted object.</returns>
        public static T ToInstance<T>(this IDictionary<string, object?> dictionary, ConverterSettings? settings = null)
            where T : new() => dictionary == null
            ? throw new ArgumentNullException(nameof(dictionary))
            : (T)ToInstanceInternal((Dictionary<string, object?>)dictionary, typeof(T), settings ?? new ConverterSettings());

        internal static object ToInstanceInternal(this Dictionary<string, object?> dictionary, Type type, ConverterSettings settings)
        {
            var instance = Activator.CreateInstance(type);
            var resolver = settings.ResolverInternal;
            var members = resolver.GetMemberInfos(type);

            foreach (var element in dictionary)
            {
                var member = members.FindMatch(element.Key, resolver);
                if (member == null)
                {
                    // Think about a 'AllKeysMustMatch' solution
                    continue;
                }

                var value = element.Value;
                var memberType = member.GetMemberType();

                if (value != null && value.GetType() == memberType)
                {
                    member.SetValue(instance, value);
                    continue;
                }

                var underlyingType = Nullable.GetUnderlyingType(memberType);
                if (value == null && underlyingType != null)
                {
                    member.SetValue(instance, null);
                    continue;
                }

                if (value != null && value.GetType() == underlyingType)
                {
                    member.SetValue(instance, value);
                    continue;
                }

                if (value != null && TypeDescriptor.GetConverter(memberType).CanConvertTo(value.GetType()))
                {
                    member.SetValue(instance, value);
                    continue;
                }

                if (memberType.IsGenericType)
                {
                    var genTypeDef = memberType.GetGenericTypeDefinition();
                    var dict = value as IDictionary<string, object?> ?? new Dictionary<string, object?>(0);

                    // Dictionary
                    if (GenericDictionaryInterfaceType.IsAssignableFrom(genTypeDef) || GenericDictionaryType.IsAssignableFrom(genTypeDef))
                    {
                        member.SetValue(instance, DefaultDictionaryConverter.Default.ToInstance(dict, memberType.GetGenericArguments()[1], settings));
                        continue;
                    }

                    // Array
                    if (EnumerableType.IsAssignableFrom(genTypeDef))
                    {
                        member.SetValue(instance, DefaultEnumerableConverter.Default.ToInstance(dict, memberType.GetGenericArguments()[0], settings));
                        continue;
                    }
                }

                var converter = settings.GetMatchingConverter(memberType);
                if (value != null && converter != null)
                {
                    member.SetValue(instance, converter.ToInstance((IDictionary<string, object?>)value, ObjectType, settings));
                    continue;
                }

                if (value is IDictionary dict2)
                {
                    member.SetValue(instance, ((Dictionary<string, object?>)dict2).ToInstanceInternal(memberType, settings));
                    continue;
                }

                throw new NotSupportedException();
            }

            return instance;
        }

        internal static MemberInfo? FindMatch(this MemberInfo[] memberInfos, string name, ISerializerResolver serializerResolver)
        {
            for (int i = 0; i < memberInfos.Length; i++)
            {
                if (memberInfos[i].GetName(serializerResolver) == name || memberInfos[i].Name == name)
                {
                    return memberInfos[i];
                }
            }

            return null;
        }
    }
}
