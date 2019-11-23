using Extensions.Dictionary.Converter;
using Extensions.Dictionary.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
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
            if (settings.TryGetMatchingConverter(type, out MemberConverter converter))
            {
                return converter.ConvertBack(dictionary, type, settings) ?? throw new InvalidOperationException();
            }

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

                if (value != null)
                {
                    // Try to use matching converter
                    if (settings.TryGetMatchingConverter(memberType, out converter))
                    {
                        member.SetValue(instance, converter.ConvertBack((IDictionary<string, object?>)value, ObjectType, settings));
                        continue;
                    }

                    // Try to convert non nullable values
                    if (value.TryConvertValue(memberType, settings, out object? conValue))
                    {
                        member.SetValue(instance, conValue);
                        continue;
                    }
                }   // Assign null if value is null and the underlying member type allows nullables
                else if (memberType.TryGetUnderlyingType(out Type _))
                {
                    member.SetValue(instance, null);
                    continue;
                }

                // Try to convert generic member
                if (TryConvertGenericValue(value, memberType, settings, out object? genValue))
                {
                    member.SetValue(instance, genValue);
                    continue;
                }

                // Recursive call if value is a dictionary
                if (value is IDictionary dict2)
                {
                    member.SetValue(instance, ((Dictionary<string, object?>)dict2).ToInstanceInternal(memberType, settings));
                    continue;
                }

                throw new NotSupportedException();
            }

            return instance;
        }

        internal static bool TryConvertGenericValue(object? initialValue, Type memberType, ConverterSettings settings, out object? value)
        {
            value = null;
            if (!memberType.IsGenericType)
            {
                return false;
            }

            var genTypeDef = memberType.GetGenericTypeDefinition();
            var dict = initialValue as IDictionary<string, object?> ?? new Dictionary<string, object?>(0);

            // Dictionary
            if (GenericDictionaryInterfaceType.IsAssignableFrom(genTypeDef) || GenericDictionaryType.IsAssignableFrom(genTypeDef))
            {
                value = DefaultDictionaryConverter.Default.ConvertBack(dict, memberType.GetGenericArguments()[1], settings);
                return true;
            }

            // Array
            if (EnumerableType.IsAssignableFrom(genTypeDef))
            {
                value = DefaultEnumerableConverter.Default.ConvertBack(dict, memberType.GetGenericArguments()[0], settings);
                return true;
            }

            return false;
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
