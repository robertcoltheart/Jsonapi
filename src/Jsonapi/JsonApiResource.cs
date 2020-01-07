using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi
{
    public class JsonApiResource<T> : JsonApiResourceIdentifier
    {
        [JsonPropertyName("attributes")]
        public T Attributes { get; set; }

        [JsonPropertyName("relationships")]
        public Dictionary<string, JsonApiRelationship> Relationships { get; set; }

        [JsonPropertyName("links")]
        public Dictionary<string, JsonApiLink> Links { get; set; }
    }

    //[JsonConverter(typeof(JsonApiResourceConverter))]
    public class JsonApiResource : JsonApiResourceIdentifier
    {
        [JsonPropertyName("attributes")]
        public JsonElement Attributes { get; set; }

        [JsonPropertyName("relationships")]
        public Dictionary<string, JsonApiRelationship> Relationships { get; set; }

        [JsonPropertyName("links")]
        public Dictionary<string, JsonApiLink> Links { get; set; }
    }
}
