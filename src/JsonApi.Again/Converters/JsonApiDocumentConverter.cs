using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    internal class JsonApiDocumentConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = JsonSerializer.Deserialize<JsonApiDocument<T>>(ref reader, options);

            return document.Data.First().Attributes;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
        }
    }
}
