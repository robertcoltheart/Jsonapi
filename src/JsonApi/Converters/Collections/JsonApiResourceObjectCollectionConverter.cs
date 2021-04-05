﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Collections
{
    internal class JsonApiResourceObjectCollectionConverter<T, TElement> : JsonApiConverter<T>
    {
        public Type? ElementType { get; } = typeof(TElement);

        public JsonTypeCategory TypeCategory { get; } = typeof(T).GetTypeCategory();

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resources = default(T);

            var state = reader.ReadDocument();
            var readState = new JsonApiState();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    resources = ReadWrapped(ref reader, ref readState, typeToConvert, default, options);
                }
                else if (name == JsonApiMembers.Included)
                {
                    ReadIncluded(ref reader, ref readState, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            return resources;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref JsonApiState state, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadArray("resources");

            var resources = new List<TElement>();

            var converter = options.GetWrappedConverter<TElement>();

            while (reader.IsInArray())
            {
                var resource = converter.ReadWrapped(ref reader, ref state, ElementType!, default, options);

                if (resource != null)
                {
                    resources.Add(resource);
                }

                reader.Read();
            }

            return (T) GetCollection(resources);
        }

        private void ReadIncluded(ref Utf8JsonReader reader, ref JsonApiState state, JsonSerializerOptions options)
        {
            reader.ReadArray("included");

            while (reader.IsInArray())
            {
                var identifier = reader.ReadAheadIdentifier();

                if (state.TryGetIncluded(identifier, out var included))
                {
                    included.Item2.Read(ref reader, ref state, included.Item3, options);
                }

                reader.Read();
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("data");

            WriteWrapped(writer, value, options);

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
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
        }

        private object GetCollection(List<TElement> resources)
        {
            return TypeCategory == JsonTypeCategory.Array
                ? resources.ToArray()
                : resources;
        }
    }
}