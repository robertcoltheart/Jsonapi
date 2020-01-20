using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    [JsonConverter(typeof(JsonApiErrorSourceConverter))]
    public sealed class JsonApiErrorSource
    {
        [JsonPropertyName("pointer")]
        public JsonApiPointer Pointer { get; set; }

        [JsonPropertyName("parameter")]
        public string Parameter { get; set; }
    }
}
