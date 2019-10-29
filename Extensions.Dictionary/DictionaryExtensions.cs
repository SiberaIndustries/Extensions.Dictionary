﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
            JsonSerializer.Deserialize<T>(JsonSerializer.SerializeToUtf8Bytes(dictionary));

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
            await JsonSerializer.SerializeAsync(ms, dictionary, null, ct).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return await JsonSerializer.DeserializeAsync<T>(ms, null, ct).ConfigureAwait(false);
        }
    }
}
