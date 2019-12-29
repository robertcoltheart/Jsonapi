using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Converters;

namespace Jsonapi
{
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public class JsonApiLink
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }
    }
}
