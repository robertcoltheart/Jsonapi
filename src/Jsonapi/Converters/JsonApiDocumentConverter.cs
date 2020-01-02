using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    internal class JsonApiDocumentConverter<T> : JsonConverter<T>
    {
        private readonly JsonApiResourceConverter resourceConverter = new JsonApiResourceConverter();

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API document");
            }

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var property = reader.GetString();

                reader.Read();

                ReadDocumentMember(ref reader, property, typeToConvert, options);

                reader.Read();
            }

            return default;
        }

        private void ReadDocumentMember(ref Utf8JsonReader reader, string property, Type typeToConvert, JsonSerializerOptions options)
        {
            if (property == JsonApiMembers.Data)
            {
                resourceConverter.Read(ref reader, typeToConvert, options);
            }
            else if (property == JsonApiMembers.Errors)
            {
                JsonSerializer.Deserialize<JsonApiError[]>(ref reader, options);
            }
            else if (property == JsonApiMembers.Meta)
            {
                JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options);
            }
            else if (property == JsonApiMembers.Version)
            {
                JsonSerializer.Deserialize<JsonApiObject>(ref reader, options);
            }
            else if (property == JsonApiMembers.Links)
            {
                JsonSerializer.Deserialize<JsonApiLinks>(ref reader, options);
            }
            else if (property == JsonApiMembers.Included)
            {
                JsonSerializer.Deserialize<JsonApiResource[]>(ref reader, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
        }
    }
}
