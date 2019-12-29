using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Extensions;

namespace Jsonapi.Serialization
{
    public abstract class JsonPropertyInfo
    {
        protected JsonPropertyInfo(PropertyInfo property, JsonConverter baseConverter, JsonSerializerOptions options)
        {
            Name = property.Name;
            PublicName = property.Name.ToCamelCase();
            PropertyType = property.PropertyType;
            BaseConverter = baseConverter;
            Options = options;
        }

        protected JsonConverter BaseConverter { get; }

        public string Name { get; }

        public string PublicName { get; }

        public Type PropertyType { get; }

        public JsonSerializerOptions Options { get; }

        public abstract bool HasGetter { get; }

        public abstract bool HasSetter { get; }

        public abstract object GetValueAsObject(object resource);

        public abstract void SetValueAsObject(object resource, object value);

        public abstract void Read(object resource, ref Utf8JsonReader reader);
    }
}
