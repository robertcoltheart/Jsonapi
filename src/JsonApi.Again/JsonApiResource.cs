using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi
{
    public class JsonApiResource<T> : JsonApiResourceIdentifier
    {
        [JsonPropertyName("attributes")]
        public T Attributes { get; set; }
    }
}
