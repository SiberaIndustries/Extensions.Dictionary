using Extensions.Dictionary.Resolver;
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
        private static readonly Type[] AllowedEnumarableTypes = { typeof(IList<>), typeof(IEnumerable<>), typeof(ICollection<>) };

        /// <summary>
        /// Converts a dictionary to the specific .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="serializerResolver">Optional serialzer resolver.</param>
        /// <returns>The converted object.</returns>
        public static T ToInstance<T>(this IDictionary<string, object?> dictionary, ISerializerResolver? serializerResolver = null)
                where T : new() => dictionary == null
                ? throw new ArgumentNullException(nameof(dictionary))
                : (T)ToInstanceInternal((Dictionary<string, object?>)dictionary, typeof(T), serializerResolver ?? DefaultResolver.Instance);

        internal static object ToInstanceInternal(this Dictionary<string, object?> dictionary, Type type, ISerializerResolver serializerResolver)
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
                    // Think about a 'AllKeysMustMatch' solution
                    continue;
                }

                var value = element.Value;
                var memberInfoType = memberInfo.GetMemberType();
                if (value != null && value.GetType() != memberInfoType)
                {
                    var dict = (Dictionary<string, object?>?)value ?? new Dictionary<string, object?>(0);
                    if (memberInfoType.IsGenericType)
                    {
                        var genericTypeDef = memberInfoType.GetGenericTypeDefinition();
                        var genericArgs = memberInfoType.GetGenericArguments();

                        if (genericTypeDef == GenericDictionaryInterfaceType)
                        {   // Is Dictionary
                            if (genericArgs[1] != ObjectType)
                            {
                                value = dict.ChangeValueType(memberInfoType);
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
                        value = ToInstanceInternal(dict, memberInfoType, serializerResolver);
                    }
                }

                memberInfo.SetValue(instance, value);
            }

            return instance;
        }

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

        private static object ConvertList(this Dictionary<string, object?>.ValueCollection items, Type type)
        {
            var array = Array.CreateInstance(type.GenericTypeArguments[0], items.Count);
            int i = 0;
            foreach (var item in items)
            {
                array.SetValue(item, i++);
            }

            return array;
        }

        private static object ChangeValueType(this Dictionary<string, object?> dict, Type type)
        {
            var dictToCast = (IDictionary)Activator.CreateInstance(GenericDictionaryType.MakeGenericType(new[] { type.GenericTypeArguments[0], type.GenericTypeArguments[1] }));
            foreach (var pair in dict)
            {
                dictToCast.Add(pair.Key, pair.Value);
            }

            return dictToCast;
        }
    }
}
