using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Serialization
{
    internal class JsonClassInfo<T>
    {
        public JsonClassInfo(JsonSerializerOptions options)
        {
            Options = options;
            Creator = Activator.CreateInstance<T>;
            Properties = GetProperties();
        }

        public JsonSerializerOptions Options { get; }

        public Func<T> Creator { get; }

        public Dictionary<string, JsonPropertyInfo<T>> Properties { get; }

        private Dictionary<string, JsonPropertyInfo<T>> GetProperties()
        {
            var comparer = Options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            return typeof(T)
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

        private JsonPropertyInfo<T> CreateProperty(PropertyInfo property)
        {
            var propertyType = typeof(ReflectionJsonPropertyInfo<,>).MakeGenericType(typeof(T), property.PropertyType);
            var converter = Options.GetConverter(property.PropertyType);

            return Activator.CreateInstance(propertyType, property, converter, Options) as JsonPropertyInfo<T>;
        }
    }
}
