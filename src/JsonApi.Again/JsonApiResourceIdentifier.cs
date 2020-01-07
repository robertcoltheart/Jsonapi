using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi
{
    public class JsonApiResourceIdentifier
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }
    }
}
