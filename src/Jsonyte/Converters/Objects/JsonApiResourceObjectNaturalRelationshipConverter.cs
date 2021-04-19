﻿using System;
using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiResourceObjectNaturalRelationshipConverter<T> : JsonApiRelationshipDetailsConverter<T>
    {
        private readonly JsonTypeInfo info;

        public JsonApiResourceObjectNaturalRelationshipConverter(JsonTypeInfo info)
        {
            this.info = info;
        }

        public override RelationshipResource<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override RelationshipResource<T> Read(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override RelationshipResource<T> ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, RelationshipResource<T> existingValue, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, ref TrackedResources tracked, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteWrapped(writer, ref tracked, value, options);

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            if (value.Resource == null)
            {
                writer.WriteNull(JsonApiMembers.DataEncoded);

                return;
            }

            var data = info.DataMember.GetValue(value.Resource);
            var meta = info.MetaMember.GetValue(value.Resource);
            var links = info.LinksMember.GetValue(value.Resource);

            if (data == null && links == null && meta == null)
            {
                writer.WriteNull(JsonApiMembers.DataEncoded);

                return;
            }

            if (data != null)
            {
                var relationshipsWritten = false;

                writer.WritePropertyName(JsonApiMembers.DataEncoded);
                info.DataMember.WriteRelationshipWrapped(writer, ref tracked, value.Resource, ref relationshipsWritten);
            }

            info.LinksMember.Write(writer, ref tracked, value.Resource);
            info.MetaMember.Write(writer, ref tracked, value.Resource);
        }
    }
}
