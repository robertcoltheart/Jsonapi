using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Converters;

namespace Jsonapi
{
    [JsonConverter(typeof(JsonApiObjectConverter))]
    public class JsonApiObject
    {
        [JsonPropertyName("version")]
        public JsonApiVersion Version { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }
    }
}
