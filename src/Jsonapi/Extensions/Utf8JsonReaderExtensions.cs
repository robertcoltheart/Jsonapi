using System.Text.Json;

namespace Jsonapi.Extensions
{
    public static class Utf8JsonReaderExtensions
    {
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
