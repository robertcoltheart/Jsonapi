using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Serialization
{
    internal class JsonClassInfo
    {
        public JsonClassInfo(Type type, JsonSerializerOptions options)
        {
            Options = options;
            Creator = () => Activator.CreateInstance(type);
            Properties = GetProperties(type);
        }

        public JsonSerializerOptions Options { get; }

        public Func<object> Creator { get; }

        public Dictionary<string, JsonPropertyInfo> Properties { get; }

        private Dictionary<string, JsonPropertyInfo> GetProperties(Type type)
        {
            var comparer = Options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => !x.GetIndexParameters().Any())
                .Where(x => x.GetMethod?.IsPublic == true || x.SetMethod?.IsPublic == true)
                .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                .ToDictionary(GetPropertyName, CreateProperty, comparer);
        }

        private string GetPropertyName(PropertyInfo property)
        {
            var nameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>(false);

            if (nameAttribute != null)
            {
                return nameAttribute.Name;
            }

            if (Options.PropertyNamingPolicy != null)
            {
                return Options.PropertyNamingPolicy.ConvertName(property.Name);
            }

            return property.Name;
        }

        private JsonPropertyInfo CreateProperty(PropertyInfo property)
        {
            var propertyType = typeof(ReflectionJsonPropertyInfo<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var converter = Options.GetConverter(property.PropertyType);

            return Activator.CreateInstance(propertyType, property, converter, Options) as JsonPropertyInfo;
        }
    }
}
