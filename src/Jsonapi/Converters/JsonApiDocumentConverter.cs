using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiDocumentConverter : JsonConverter<JsonApiDocument>
    {
        public override JsonApiDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API document");
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, JsonApiDocument value, JsonSerializerOptions options)
        {
        }
    }

    internal class JsonApiDocumentConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API document");
            }

            var dataType = typeToConvert.GetGenericArguments()[0];

            var root = Activator.CreateInstance(typeof(JsonApiDocument<>).MakeGenericType(dataType));

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var property = reader.GetString();

                reader.Read();

                if (property == JsonApiMembers.Data)
                {
                    var data = JsonSerializer.Deserialize(ref reader, dataType, options);

                    var dataProperty = root.GetType().GetProperty("Data", BindingFlags.Instance | BindingFlags.Public);
                    dataProperty?.SetValue(root, data);
                }
                else if (property == JsonApiMembers.Errors)
                {
                    var errors = JsonSerializer.Deserialize<JsonApiError[]>(ref reader, options);

                    var errorsProperty = root.GetType().GetProperty("Errors", BindingFlags.Instance | BindingFlags.Public);
                    errorsProperty?.SetValue(root, errors);
                }
                else
                {
                    ReadDocumentMember(ref reader, property, typeToConvert, options);
                }

                reader.Read();
            }

            return (T) root;
        }

        private void ReadDocumentMember(ref Utf8JsonReader reader, string property, Type typeToConvert, JsonSerializerOptions options)
        {
            if (property == JsonApiMembers.Data)
            {
                //resourceConverter.Read(ref reader, typeToConvert, options);
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
