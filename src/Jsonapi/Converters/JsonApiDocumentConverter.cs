using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiDocumentConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API document");
            }

            var documentInfo = options.GetClassInfo(typeToConvert);

            var root = documentInfo.Creator();

            reader.Read();

            var flags = DocumentFlags.None;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var property = reader.GetString();

                AddFlag(ref flags, property);

                reader.Read();

                if (documentInfo.Properties.TryGetValue(property, out var propertyInfo))
                {
                    if (property == JsonApiMembers.Data)
                    {
                        var dataResourceType = typeof(JsonApiResource<>).MakeGenericType(propertyInfo.PropertyType);
                        var dataResourceInfo = options.GetClassInfo(dataResourceType);
                        var dataInfo = options.GetClassInfo(propertyInfo.PropertyType);

                        var dataIdentifier = (JsonApiResourceIdentifier) JsonSerializer.Deserialize(ref reader, dataResourceType, options);

                        var data = dataResourceInfo.Properties["attributes"].GetValueAsObject(dataIdentifier);

                        if (dataInfo.Properties.TryGetValue("id", out var id))
                        {
                            id.SetValueAsObject(data, dataIdentifier.Id);
                        }

                        if (dataInfo.Properties.TryGetValue("type", out var type))
                        {
                            type.SetValueAsObject(data, dataIdentifier.Type);
                        }

                        propertyInfo.SetValueAsObject(root, data);
                    }
                    else if (propertyInfo.HasConverter)
                    {
                        propertyInfo.Read(root, ref reader);
                    }
                    else
                    {
                        var item = JsonSerializer.Deserialize(ref reader, propertyInfo.PropertyType, options);

                        propertyInfo.SetValueAsObject(root, item);
                    }
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            ValidateFlags(flags);

            return (T) root;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
        }

        private void ValidateFlags(DocumentFlags flags)
        {
            if (!flags.HasFlag(DocumentFlags.Data) &&
                !flags.HasFlag(DocumentFlags.Errors) &&
                !flags.HasFlag(DocumentFlags.Meta))
            {
                throw new JsonApiException("JSON:API document must contain 'data', 'errors' or 'meta' members");
            }

            if (flags.HasFlag(DocumentFlags.Data) && flags.HasFlag(DocumentFlags.Errors))
            {
                throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (flags.HasFlag(DocumentFlags.Included) && !flags.HasFlag(DocumentFlags.Data))
            {
                throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }

        private void AddFlag(ref DocumentFlags flags, string name)
        {
            switch (name)
            {
                case JsonApiMembers.Data:
                    flags |= DocumentFlags.Data;
                    break;

                case JsonApiMembers.Errors:
                    flags |= DocumentFlags.Errors;
                    break;

                case JsonApiMembers.Meta:
                    flags |= DocumentFlags.Meta;
                    break;

                case JsonApiMembers.Version:
                    flags |= DocumentFlags.Jsonapi;
                    break;

                case JsonApiMembers.Links:
                    flags |= DocumentFlags.Links;
                    break;

                case JsonApiMembers.Included:
                    flags |= DocumentFlags.Included;
                    break;
            }
        }
    }
}
