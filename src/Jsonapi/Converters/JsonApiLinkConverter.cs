using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    public class JsonApiLinkConverter : JsonConverter<JsonApiLink>
    {
        public override JsonApiLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiLink value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
