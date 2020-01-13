﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsDocument())
            {
                return true;
            }

            return typeToConvert.IsResource();
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert.IsDocument())
            {
                return CreateConverter(typeof(JsonApiDocumentConverter<>), typeToConvert);
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
