using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Extensions;

namespace Jsonapi.Converters
{
    internal class JsonApiResourceConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = Activator.CreateInstance(typeToConvert);

            while (reader.TryReadMember(out var member))
            {
                if (member == JsonApiMembers.Data)
                {

                }
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
