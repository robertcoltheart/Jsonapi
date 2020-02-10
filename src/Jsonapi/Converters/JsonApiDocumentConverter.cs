using System;
using System.Collections.Generic;
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

            var documentInfo = options.GetClassInfo(typeToConvert);

            var root = documentInfo.Creator() as JsonApiDocument;

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var property = reader.GetString();

                reader.Read();

                if (documentInfo.Properties.TryGetValue(property, out var propertyInfo))
                {
                    var item = JsonSerializer.Deserialize(ref reader, propertyInfo.PropertyType, options);

                    propertyInfo.SetValueAsObject(root, item);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            return root;
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
            var documentType = typeof(JsonApiDocument<>).MakeGenericType(dataType);

            var documentInfo = options.GetClassInfo(documentType);

            var root = documentInfo.Creator();

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

                    documentInfo.Properties[JsonApiMembers.Data].SetValueAsObject(root, data);
                }
                else if (property == JsonApiMembers.Errors)
                {
                    var errors = JsonSerializer.Deserialize<JsonApiError[]>(ref reader, options);

                    documentInfo.Properties[JsonApiMembers.Errors].SetValueAsObject(root, errors);
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
