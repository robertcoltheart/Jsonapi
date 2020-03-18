using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
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
                throw new JsonApiException("Invalid JSON:API document, expected an object");
            }

            reader.Read();

            var document = new JsonApiDocument();

            while (reader.TokenType == JsonTokenType.PropertyName)
            {
                var name = reader.GetString();

                reader.Read();

                if (name == JsonApiMembers.Data)
                {
                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        document.Data = new[] {JsonSerializer.Deserialize<JsonApiResource>(ref reader, options)};
                    }
                    else if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        document.Data = JsonSerializer.Deserialize<JsonApiResource[]>(ref reader, options);
                    }
                    else
                    {
                        throw new JsonApiException("Invalid data element in JSON:API document, expected object or array");
                    }
                }
                else if (name == JsonApiMembers.Errors)
                {
                    document.Errors = JsonSerializer.Deserialize<JsonApiError[]>(ref reader, options);
                }
                else if (name == JsonApiMembers.Meta)
                {
                    document.Meta = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options);
                }
                else if (name == JsonApiMembers.Jsonapi)
                {
                    document.Object = JsonSerializer.Deserialize<JsonApiObject>(ref reader, options);
                }
                else if (name == JsonApiMembers.Links)
                {
                    document.Links = JsonSerializer.Deserialize<JsonApiLinks>(ref reader, options);
                }
                else if (name == JsonApiMembers.Included)
                {
                    document.Included = JsonSerializer.Deserialize<JsonApiResource[]>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            var att = document.Included[0].Attributes.Values.First();

            var writer = new Utf8JsonWriter(new IBufferWriter<>);
            att.WriteTo(writer);

            JsonDocument doc;

            JsonSerializer.Deserialize<Type>(writer.writte)

            return document;
        }

        public override void Write(Utf8JsonWriter writer, JsonApiDocument value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteEndObject();
        }
    }
}
