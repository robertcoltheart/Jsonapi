using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiErrorSourceConverter : JsonConverter<JsonApiErrorSource>
    {
        public override JsonApiErrorSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Error source must be an object");
            }

            return reader.ReadObject<JsonApiErrorSource>(options);
        }

        public override void Write(Utf8JsonWriter writer, JsonApiErrorSource value, JsonSerializerOptions options)
        {
            if (value.Pointer != null)
            {
                writer.WriteString("pointer", value.Pointer.ToString() ?? string.Empty);
            }

            if (!string.IsNullOrEmpty(value.Parameter))
            {
                writer.WriteString("parameter", value.Parameter);
            }
        }
    }
}
