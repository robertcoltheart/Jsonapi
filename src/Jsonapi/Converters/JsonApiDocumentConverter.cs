using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Extensions;
using Jsonapi.Serialization;

namespace Jsonapi.Converters
{
    internal class JsonApiDocumentConverter<T> : JsonConverter<T>
    {
        public JsonApiDocumentConverter(JsonSerializerOptions options)
        {
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid object");
            }

            reader.Read();

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
        }
    }
}
