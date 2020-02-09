﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    public class JsonApiErrorsConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiDocument<T>>(ref reader, options);

                return (T) (object) document.Errors;
            }

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiException("Invalid JSON:API errors array");
            }

            reader.Read();

            var list = new List<JsonApiError>();

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                //var error = reader.ReadObject<JsonApiError>(options);

                var error = JsonSerializer.Deserialize<JsonApiError>(ref reader, options);

                list.Add(error);

                reader.Read();
            }

            if (typeToConvert.IsArray)
            {
                return (T) (object) list.ToArray();
            }

            return (T) (object) list;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var errors = value as IEnumerable<JsonApiError> ?? Enumerable.Empty<JsonApiError>();

            writer.WriteStartArray();

            foreach (var error in errors)
            {
                JsonSerializer.Serialize(writer, error, options);
            }

            writer.WriteEndArray();
        }
    }
}
