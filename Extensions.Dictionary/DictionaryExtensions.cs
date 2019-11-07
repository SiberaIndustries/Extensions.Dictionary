using Extensions.Dictionary.Resolver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private static readonly Type EnumerableType = typeof(Enumerable);
        private static readonly MethodInfo EnumerableCast = EnumerableType.GetMethod(nameof(Enumerable.Cast));
        private static readonly MethodInfo EnumerableToList = EnumerableType.GetMethod(nameof(Enumerable.ToList));
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

        internal static object ToInstanceInternal(this IDictionary<string, object?> dictionary, Type type, ISerializerResolver serializerResolver, JsonSerializerOptions? options)
        {
            var instance = Activator.CreateInstance(type);
            var memberInfos = serializerResolver.GetMemberInfos(type);

            foreach (var element in dictionary)
            {
                var memberInfo = memberInfos.SingleOrDefault(x => x.GetName(serializerResolver) == element.Key || x.Name == element.Key);
                if (memberInfo == null)
                {
                    // TODO: Think about a 'AllKeysMustMatch' solution
                    // throw new NotSupportedException($"Property or field '{type.Name}.{element.Key}' could not be found");
                    continue;
                }

                object? value = null;
                var memberInfoType = memberInfo.GetMemberType();
                var valueType = element.Value != null
                    ? element.Value?.GetType()
                    : memberInfoType;

                if (memberInfoType.IsGenericType)
                {
                    var genericTypeDef = memberInfoType.GetGenericTypeDefinition();
                    var genericArgs = memberInfoType.GetGenericArguments();

                    // Is Dictionary
                    if (genericTypeDef == GenericDictionaryType)
                    {
                        value = genericArgs[1] == ObjectType
                            ? element.Value
                            : ToInstanceFallback(element.Value, memberInfoType, options); // TODO: How to avoid this?
                    }

                    // Is Enumerable
                    if (AllowedEnumarableTypes.Contains(genericTypeDef))
                    {
                        var dict = (IDictionary<string, object>?)element.Value ?? new Dictionary<string, object>(0);
                        value = dict.Values.ConvertList(memberInfoType);
                    }
                }
                else if (memberInfoType == valueType)
                {
                    value = element.Value;
                }
                else
                {
                    try
                    {
                        value = element.Value != null
                            ? ToInstanceInternal((IDictionary<string, object?>)element.Value, memberInfoType, serializerResolver, options)
                            : element.Value;
                    }
                    catch (InvalidCastException)
                    {
                        value = ToInstanceFallback(element.Value, memberInfoType, options); // TODO: How to avoid this?
                    }
                    catch (InvalidOperationException)
                    {
                        value = ToInstanceFallback(element.Value, memberInfoType, options); // TODO: How to avoid this?
                    }
                }

                memberInfo.SetValue(instance, value);
            }

            return instance;
        }

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

        private static object ConvertList(this IEnumerable<object> items, Type type, bool convert = false)
        {
            var containedType = type.GenericTypeArguments[0];
            var itemsToCast = convert
                ? items.Select(item => Convert.ChangeType(item, containedType, CultureInfo.InvariantCulture))
                : items;

            var castedItems = EnumerableCast.MakeGenericMethod(containedType).Invoke(null, new[] { itemsToCast });
            return EnumerableToList.MakeGenericMethod(containedType).Invoke(null, new[] { castedItems });
        }

        /// <summary>
        /// Converts a dictionary to the specific .NET type asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="converters">Additional custom converter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The converted object.</returns>
        [Obsolete("This method will soon be deprecated. Use " + nameof(ToInstance) + " instead.")]
        public static async Task<T> ToInstanceAsync<T>(this IDictionary<string, object?> dictionary, IEnumerable<JsonConverter>? converters = null, CancellationToken ct = default)
        {
            using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, dictionary, null, ct).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return await JsonSerializer.DeserializeAsync<T>(ms, GetSerializerOptions(converters)).ConfigureAwait(false);
        }
    }
}
