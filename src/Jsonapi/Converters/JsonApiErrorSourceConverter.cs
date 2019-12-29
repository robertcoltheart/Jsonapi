using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    public class JsonApiErrorSourceConverter : JsonConverter<JsonApiErrorSource>
    {
        public override JsonApiErrorSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiErrorSource value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
