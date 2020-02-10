﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiLinkConverter : JsonConverter<JsonApiLink>
    {
        public override JsonApiLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var link = new JsonApiLink();

            if (reader.TokenType == JsonTokenType.String)
            {
                link.Href = reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Read();

                while (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var name = reader.GetString();

                    reader.Read();

                    if (name == "href")
                    {
                        link.Href = reader.GetString();
                    }
                    else if (name == "meta")
                    {
                        link.Meta = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options);
                    }
                    else
                    {
                        reader.Skip();
                    }

                    reader.Read();
                }
            }

            return link;
        }

        public override void Write(Utf8JsonWriter writer, JsonApiLink value, JsonSerializerOptions options)
        {
            if (value.Meta == null || !value.Meta.Any())
            {
                writer.WriteStringValue(value.Href ?? string.Empty);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteString("href", value.Href ?? string.Empty);

                writer.WriteStartObject();

                foreach (var element in value.Meta)
                {
                    writer.WritePropertyName(element.Key);
                    element.Value.WriteTo(writer);
                }

                writer.WriteEndObject();
                writer.WriteEndObject();
            }
        }
    }
}
