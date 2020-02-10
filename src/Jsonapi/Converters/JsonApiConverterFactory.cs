using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsError())
            {
                return false;
            }

            if (typeToConvert.IsCollection() && typeToConvert.GetCollectionType() == typeof(JsonApiError))
            {
                return true;
            }

            if (typeToConvert.IsDocument())
            {
                return true;
            }

            return typeToConvert.IsResource();
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert.IsCollection() && typeToConvert.GetCollectionType() == typeof(JsonApiError))
            {
                return CreateConverter(typeof(JsonApiErrorsConverter<>), typeToConvert);
            }

            if (typeToConvert.IsDocument())
            {
                return typeToConvert == typeof(JsonApiDocument)
                    ? new JsonApiDocumentConverter()
                    : CreateConverter(typeof(JsonApiDocumentConverter<>), typeToConvert);
            }

            return CreateConverter(typeof(JsonApiResourceConverter<>), typeToConvert);
        }

        private JsonConverter CreateConverter(Type converterType, Type typeToConvert)
        {
            var genericType = converterType.MakeGenericType(typeToConvert);

            return Activator.CreateInstance(genericType) as JsonConverter;
        }
    }
}
