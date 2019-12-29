using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    public class JsonApiRelationshipConverter : JsonConverter<JsonApiRelationship>
    {
        public override JsonApiRelationship Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiRelationship value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
