﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    public class JsonApiObjectConverter : JsonConverter<JsonApiObject>
    {
        public override JsonApiObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiObject value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
