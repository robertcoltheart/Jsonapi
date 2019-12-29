using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Converters;

namespace Jsonapi
{
    //[JsonConverter(typeof(JsonApiDocumentConverter))]
    public class JsonApiDocument
    {
        [JsonPropertyName("data")]
        public JsonApiResource[] Data { get; set; }

        [JsonPropertyName("errors")]
        public JsonApiError Errors { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }

        [JsonPropertyName("jsonapi")]
        public JsonApiObject Version { get; set; }

        [JsonPropertyName("links")]
        public JsonApiLinks Links { get; set; }

        [JsonPropertyName("included")]
        public JsonApiResource[] Included { get; set; }
    }
}
