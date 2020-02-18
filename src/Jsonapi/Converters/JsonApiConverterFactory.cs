using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsDocument();
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return CreateConverter(typeof(JsonApiDocumentConverter<>), typeToConvert);
        }

        private JsonConverter CreateConverter(Type converterType, Type typeToConvert)
        {
            var genericType = converterType.MakeGenericType(typeToConvert);

            return Activator.CreateInstance(genericType) as JsonConverter;
        }
    }
}
