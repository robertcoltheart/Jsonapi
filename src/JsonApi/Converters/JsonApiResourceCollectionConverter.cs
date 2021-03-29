﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourceCollectionConverter<T, TElement> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resources = default(T);

            var state = reader.ReadDocument();

            while (reader.IsObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    resources = ReadResources(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            return resources;
        }

        private T? ReadResources(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadArray("resources");

            var resources = new List<TElement>();

            var converter = options.GetWrappedConverter<TElement>();

            while (reader.IsArray())
            {
                var resource = converter.ReadWrapped(ref reader, typeof(TElement), options);

                if (resource != null)
                {
                    resources.Add(resource);
                }

                reader.Read();
            }

            return (T) GetInstance(resources);
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("data");

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else if (value is IEnumerable<TElement> collection)
            {
                var converter = options.GetWrappedConverter<TElement>();

                writer.WriteStartArray();

                foreach (var element in collection)
                {
                    converter.WriteWrapped(writer, element, options);
                }

                writer.WriteEndArray();
            }
            else
            {
                throw new JsonApiException("JSON:API resources collection must be an enumerable");
            }

            writer.WriteEndObject();
        }

        private object GetInstance(List<TElement> resources)
        {
            var category = typeof(T).GetTypeCategory();

            if (category == JsonTypeCategory.Array)
            {
                return resources.ToArray();
            }

            return resources;
        }
    }
}
