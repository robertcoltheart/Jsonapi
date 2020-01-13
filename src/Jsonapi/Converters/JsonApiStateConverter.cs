using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiStateConverter : JsonConverter<JsonApiStateConverter>
    {
        private readonly ConcurrentDictionary<Type, JsonClassInfo> classInfos = new ConcurrentDictionary<Type,JsonClassInfo>();

        public JsonClassInfo GetClassInfo(JsonSerializerOptions options, Type type)
        {
            return classInfos.GetOrAdd(type, x => new JsonClassInfo(x, options));
        }

        public override JsonApiStateConverter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiStateConverter value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
