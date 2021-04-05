﻿using System;
using System.Linq;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiResourceObjectConverter<T> : JsonApiConverter<T>
    {
        public Type TypeToConvert { get; } = typeof(T);

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resource = default(T);

            var state = reader.ReadDocument();
            var readState = new JsonApiState();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    resource = ReadWrapped(ref reader, ref readState, typeToConvert, default, options);
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

            state.Validate();

            return resource;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref JsonApiState state, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var resourceState = reader.ReadResource();

            var info = options.GetClassInfo(typeToConvert);
            var resource = existingValue ?? info.Creator();

            ValidateResource(info);

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref resourceState);

                var property = info.GetMember(name);

                if (name == JsonApiMembers.Id || name == JsonApiMembers.Type)
                {
                    property.Read(ref reader, resource);
                }
                else if (name == JsonApiMembers.Attributes)
                {
                    reader.ReadObject("resource attributes");

                    while (reader.IsInObject())
                    {
                        var attributeName = reader.ReadMember("resource");

                        info.GetMember(attributeName).Read(ref reader, resource);

                        reader.Read();
                    }
                }
                else if (name == JsonApiMembers.Meta)
                {
                    property.Read(ref reader, resource);
                }
                else if (name == JsonApiMembers.Relationships)
                {
                    ReadRelationships(ref reader, ref state, resource, info);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            resourceState.Validate();

            return (T) resource;
        }

        private void ReadRelationships(ref Utf8JsonReader reader, ref JsonApiState state, object resource, JsonTypeInfo info)
        {
            reader.ReadObject("relationship");

            while (reader.IsInObject())
            {
                var relationshipName = reader.ReadMember("relationship");

                info.GetMember(relationshipName).ReadRelationship(ref reader, ref state, resource);

                reader.Read();
            }
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

                return;
            }

            var info = options.GetClassInfo(TypeToConvert);

            ValidateResource(info);

            var comparer = options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            var valueKeys = info.GetMemberKeys()
                .Except(new[] {JsonApiMembers.Id, JsonApiMembers.Type, JsonApiMembers.Meta}, comparer)
                .ToArray();

            writer.WriteStartObject();

            info.GetMember(JsonApiMembers.Id).Write(writer, value);
            info.GetMember(JsonApiMembers.Type).Write(writer, value);
            
            if (valueKeys.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Attributes);
                writer.WriteStartObject();

                foreach (var key in valueKeys)
                {
                    info.GetMember(key).Write(writer, value);
                }

                writer.WriteEndObject();
            }

            info.GetMember(JsonApiMembers.Meta).Write(writer, value);

            writer.WriteEndObject();
        }

        protected void ValidateResource(JsonTypeInfo info)
        {
            var idProperty = info.GetMember(JsonApiMembers.Id);

            if (!string.IsNullOrEmpty(idProperty.Name) && idProperty.MemberType != typeof(string))
            {
                throw new JsonApiException("JSON:API resource id must be a string");
            }

            var typeProperty = info.GetMember(JsonApiMembers.Type);

            if (string.IsNullOrEmpty(typeProperty.Name))
            {
                throw new JsonApiException("JSON:API resource must have a 'type' member");
            }

            if (typeProperty.MemberType != typeof(string))
            {
                throw new JsonApiException("JSON:API resource type must be a string");
            }
        }
    }
}