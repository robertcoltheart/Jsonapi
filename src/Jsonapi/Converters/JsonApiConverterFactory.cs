using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Extensions;

namespace Jsonapi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsResource();
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(JsonApiDocumentConverter<>).MakeGenericType(typeToConvert);

            return Activator.CreateInstance(converterType) as JsonConverter;
        }
    }
}
