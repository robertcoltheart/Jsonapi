﻿using System;
using System.Text.Json;

namespace JsonApi.Converters
{
    internal abstract class JsonApiRelationshipDetailsConverter<T> : WrappedJsonConverter<T>
    {
        public abstract T? Read(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonSerializerOptions options);
    }
}