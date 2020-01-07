using System;
using System.Reflection;
using System.Text.Json;

namespace Jsonapi.Serialization
{
    internal abstract class JsonPropertyInfo
    {
        protected JsonPropertyInfo(PropertyInfo property, JsonSerializerOptions options)
        {
            Name = property.Name;
            PublicName = property.Name.ToCamelCase();
            PropertyType = property.PropertyType;
            Options = options;
        }

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
