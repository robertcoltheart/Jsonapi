using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Extensions;
using Jsonapi.Serialization;

namespace Jsonapi.Converters
{
    public class JsonApiDocumentConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resource = Activator.CreateInstance<T>();

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid object");
            }

            reader.Read();

            while (reader.TryReadMember(out var documentMember))
            {
                if (documentMember == JsonApiMembers.Data)
                {
                    if (reader.TokenType == JsonTokenType.Null)
                    {
                        return default;
                    }

                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        throw new JsonApiException("Invalid data");
                    }

                    reader.Read();

                    while (reader.TryReadMember(out var dataMember))
                    {
                        var populated = PopulateResource(resource, dataMember, ref reader, options);

                        if (!populated && dataMember == JsonApiMembers.Attributes)
                        {
                            if (reader.TokenType != JsonTokenType.StartObject)
                            {
                                throw new JsonApiException("Invalid data");
                            }

                            reader.Read();

                            while (reader.TryReadMember(out var attributeMember))
                            {
                                PopulateResource(resource, attributeMember, ref reader, options);
                            }
                        }
                    }
                }
            }

            return resource;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private bool PopulateResource(object resource, string member, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var property = GetProperty(member, options);

            if (property == null)
            {
                return false;
            }

            property.Read(resource, ref reader);

            return true;
        }

        private JsonPropertyInfo GetProperty(string name, JsonSerializerOptions options)
        {
            var property = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                var propertyType = typeof(ReflectionJsonPropertyInfo<,>).MakeGenericType(typeof(T), property.PropertyType);
                var converter = options.GetConverter(property.PropertyType);
                var reflectionProperty = Activator.CreateInstance(propertyType, property, converter, options);

                return reflectionProperty as JsonPropertyInfo;
            }

            return null;
        }
    }
}
