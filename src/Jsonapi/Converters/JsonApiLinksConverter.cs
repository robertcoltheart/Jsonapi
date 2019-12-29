using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    public class JsonApiLinksConverter : JsonConverter<JsonApiLinks>
    {
        public override JsonApiLinks Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiLinks value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
