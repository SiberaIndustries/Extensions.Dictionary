using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.Dictionary
{
    public static class DictionaryExtensions
    {
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

        /// <summary>
        /// Converts a dictionary to the specific .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="converters">Additional custom converter.</param>
        /// <returns>The converted object.</returns>
        public static T ToInstance<T>(this IDictionary<string, object?> dictionary, IEnumerable<JsonConverter>? converters = null) =>
            JsonSerializer.Deserialize<T>(JsonSerializer.SerializeToUtf8Bytes(dictionary), GetSerializerOptions(converters));

        /// <summary>
        /// Converts a dictionary to the specific .NET type asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="converters">Additional custom converter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The converted object.</returns>
        public static async Task<T> ToInstanceAsync<T>(this IDictionary<string, object?> dictionary, IEnumerable<JsonConverter>? converters = null, CancellationToken ct = default)
        {
            using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, dictionary, null, ct).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return await JsonSerializer.DeserializeAsync<T>(ms, GetSerializerOptions(converters)).ConfigureAwait(false);
        }
    }
}
