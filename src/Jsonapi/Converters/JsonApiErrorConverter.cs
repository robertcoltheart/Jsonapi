using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiErrorConverter : JsonConverter<JsonApiError>
    {
        public override JsonApiError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiDocument>(ref reader, options);

                return document.Errors.FirstOrDefault();
            }

            return reader.ReadObject<JsonApiError>(options);
        }

        public override void Write(Utf8JsonWriter writer, JsonApiError value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
