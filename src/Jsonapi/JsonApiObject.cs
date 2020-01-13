using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    public class JsonApiObject
    {
        [JsonPropertyName("version")]
        public JsonApiVersion Version { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }
    }
}
