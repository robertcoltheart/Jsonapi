using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    public class JsonApiErrorsConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var type = options.GetClassInfo(typeToConvert);

            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiDocument>(ref reader, options);

                return (T) GetInstance(type, document.Errors);
            }

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiException("Invalid JSON:API errors array");
            }

            reader.Read();

            var list = new List<JsonApiError>();

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var error = JsonSerializer.Deserialize<JsonApiError>(ref reader, options);

                list.Add(error);

                reader.Read();
            }

            return (T) GetInstance(type, list.ToArray());
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

        private object GetInstance(JsonClassInfo info, JsonApiError[] errors)
        {
            if (info.ClassType == JsonClassType.Array)
            {
                return errors;
            }

            if (info.ClassType == JsonClassType.List)
            {
                var list = info.Creator() as IList<JsonApiError>;

                foreach (var error in errors)
                {
                    list.Add(error);
                }

                return list;
            }

            return errors;
        }
    }
}
