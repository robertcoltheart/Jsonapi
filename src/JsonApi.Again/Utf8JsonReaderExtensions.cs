using System.Text.Json;

namespace Jsonapi
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
    }
}
