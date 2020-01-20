using System;
using System.Text.Json;

namespace JsonApi
{
    internal static class Utf8JsonReaderExtensions
    {
        public static bool IsDocument(this Utf8JsonReader reader)
        {
            if (reader.CurrentDepth > 0)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.None)
            {
                if (!reader.Read())
                {
                    return false;
                }
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return false;
            }

            if (!reader.Read())
            {
                return false;
            }

            while (reader.TokenType == JsonTokenType.PropertyName)
            {
                var name = reader.GetString();

                if (name == "data" || name == "errors" || name == "meta")
                {
                    return true;
                }

                if (!reader.TrySkip())
                {
                    return false;
                }

                if (!reader.Read())
                {
                    return false;
                }
            }

            return false;
        }

        public static T ReadObject<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return (T) ReadObject(ref reader, typeof(T), options);
        }

        public static object ReadObject(this ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API resource");
            }

            var info = options.GetClassInfo(typeToConvert);

            var resource = info.Creator();

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var name = reader.GetString();

                reader.Read();

                if (info.Properties.TryGetValue(name, out var property))
                {
                    property.Read(resource, ref reader);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            return resource;
        }

        public static bool TryReadMember(this ref Utf8JsonReader reader, out string name)
        {
            if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.EndObject)
            {
                name = default;

                return false;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonApiException("Expected property name");
            }

            name = reader.GetString();

            reader.Read();

            return true;
        }
    }
}
