﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters.Collections;
using JsonApi.Converters.Objects;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        private static readonly HashSet<Type> IgnoredTypes = new()
        {
            typeof(JsonApiDocumentLinks),
            typeof(JsonApiErrorLinks),
            typeof(JsonApiErrorSource),
            typeof(JsonApiLinks),
            typeof(JsonApiMeta),
            typeof(JsonApiObject),
            typeof(JsonApiRelationshipLinks),
            typeof(JsonApiResource),
            typeof(JsonApiResourceIdentifier),
            typeof(JsonApiResourceLinks),
            typeof(JsonApiResourceIdentifier[]),
            typeof(JsonElement),
            typeof(string),
            typeof(Dictionary<string, JsonElement>),
            typeof(Dictionary<string, JsonApiRelationship>)
        };

        private static readonly Dictionary<Type, JsonConverter> JsonApiConverters = new()
        {
            {typeof(JsonApiError), new JsonApiErrorConverter()},
            {typeof(JsonApiDocument), new JsonApiDocumentConverter()},
            {typeof(JsonApiLink), new JsonApiLinkConverter()},
            {typeof(JsonApiPointer), new JsonApiPointerConverter()},
            {typeof(JsonApiRelationship), new JsonApiRelationshipConverter()},
            {typeof(JsonApiResource[]), new JsonApiResourcesConverter()},
            {typeof(JsonApiError[]), new JsonApiErrorsConverter<JsonApiError[]>()},
            {typeof(List<JsonApiError>), new JsonApiErrorsConverter<List<JsonApiError>>()}
        };

        protected bool IsIgnoredType(Type typeToConvert)
        {
            if (typeToConvert.IsPrimitive)
            {
                return true;
            }

            return IgnoredTypes.Contains(typeToConvert);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (IsIgnoredType(typeToConvert))
            {
                return false;
            }

            if (JsonApiConverters.ContainsKey(typeToConvert))
            {
                return true;
            }

            if (typeToConvert.IsDocument())
            {
                return true;
            }

            if (typeToConvert.IsCollection())
            {
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType != null && collectionType.IsError())
                {
                    return true;
                }
            }

            return false;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonApiConverters.TryGetValue(typeToConvert, out var converter))
            {
                return converter;
            }

            if (typeToConvert.IsDocument())
            {
                return CreateConverter(typeof(JsonApiDocumentConverter<>), typeToConvert.GenericTypeArguments.First());
            }

            if (typeToConvert.IsCollection())
            {
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType != null && collectionType.IsError())
                {
                    return CreateConverter(typeof(JsonApiErrorsConverter<>), typeToConvert);
                }
            }

            return null;
        }

        protected JsonConverter? CreateConverter(Type converterType, params Type[] typesToConvert)
        {
            var genericType = converterType.MakeGenericType(typesToConvert);

            return Activator.CreateInstance(genericType) as JsonConverter;
        }
    }
}
