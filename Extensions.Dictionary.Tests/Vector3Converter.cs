using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Extensions.Dictionary.Tests
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var conv = (JsonConverter<JsonElement>)options.GetConverter(typeof(JsonElement));
            var el = conv.Read(ref reader, typeToConvert, options);
            return new Vector3(el.GetProperty("X").GetSingle(), el.GetProperty("Y").GetSingle(), el.GetProperty("Z").GetSingle());
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) =>
            throw new NotImplementedException();
    }
}
