using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonapi.Converters;

namespace Jsonapi
{
    public class JsonApiDocument<T>
    {
        [JsonPropertyName("data")]
        [JsonApiResourceConverter]
        public JsonApiResource<T>[] Data { get; set; }
    }
}
