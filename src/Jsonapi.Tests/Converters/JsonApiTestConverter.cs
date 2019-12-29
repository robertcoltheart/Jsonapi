using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Tests.Models;

namespace Jsonapi.Tests.Converters
{
    public class JsonApiTestConverter : JsonConverter<Article>
    {
        public override Article Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Article value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
