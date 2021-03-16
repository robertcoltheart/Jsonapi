﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal abstract class JsonApiConverter<T> : JsonConverter<T>
    {
        public abstract T? ReadWrapped(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

        public abstract void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
    }
}