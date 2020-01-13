using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    public class JsonApiErrorSource
    {
        [JsonPropertyName("pointer")]
        public JsonApiPointer Pointer { get; set; }

        [JsonPropertyName("parameter")]
        public string Parameter { get; set; }
    }
}
