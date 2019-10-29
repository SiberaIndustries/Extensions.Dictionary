using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace Extensions.Dictionary
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Converts a dictionary to the specific .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>The converted object.</returns>
        public static T ToInstance<T>(this IDictionary<string, object?> dictionary) =>
            JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(dictionary));

        /// <summary>
        /// Converts a dictionary to the specific .NET type asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert to.</typeparam>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The converted object.</returns>
        public static async Task<T> ToInstanceAsync<T>(this IDictionary<string, object?> dictionary, CancellationToken ct = default)
        {
            using var ms = new MemoryStream();
            ct.ThrowIfCancellationRequested();
            await JsonSerializer.SerializeAsync(ms, dictionary, null).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            ct.ThrowIfCancellationRequested();
            return await JsonSerializer.DeserializeAsync<T>(ms, null).ConfigureAwait(false);
        }
    }
}
