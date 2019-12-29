using System.Text.Json.Serialization;
using Jsonapi.Converters;

namespace Jsonapi
{
    [JsonConverter(typeof(JsonApiErrorSourceConverter))]
    public class JsonApiErrorSource
    {
        [JsonPropertyName("pointer")]
        public JsonApiPointer Pointer { get; set; }

        [JsonPropertyName("parameter")]
        public string Parameter { get; set; }
    }
}
