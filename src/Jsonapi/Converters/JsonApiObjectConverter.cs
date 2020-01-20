using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiObjectConverter : JsonConverter<JsonApiObject>
    {
        public override JsonApiObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API object");
            }

            reader.Read();

            var jsonApi = new JsonApiObject();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var name = reader.GetString();

                reader.Read();

                switch (name)
                {
                    case "version":
                        //jsonApi.Version = GetVersion(reader.GetString());
                        break;

                    case "meta":
                        jsonApi.Meta = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options);
                        break;
                }
            }

            return jsonApi;
        }

        public override void Write(Utf8JsonWriter writer, JsonApiObject value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private JsonApiVersion GetVersion(string value)
        {
            if (value == "1.0")
            {
                return JsonApiVersion.Version1_0;
            }

            if (value == "1.1")
            {
                return JsonApiVersion.Version1_1;
            }

            throw new JsonApiException($"Invalid JSON:API version: '{value}'");
        }
    }
}
