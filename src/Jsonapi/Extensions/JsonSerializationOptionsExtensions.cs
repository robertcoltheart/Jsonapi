using System;
using System.Text.Json;
using Jsonapi.Converters;
using Jsonapi.Serialization;

namespace Jsonapi.Extensions
{
    public static class JsonSerializationOptionsExtensions
    {
        public static void AddJsonApi(this JsonSerializerOptions options)
        {
            options.Converters.Add(new JsonApiStateConverter());
            options.Converters.Add(new JsonApiConverterFactory());
        }

        internal static JsonClassInfo GetClassInfo(this JsonSerializerOptions options, Type type)
        {
            return options.GetState().GetClassInfo(options, type);
        }

        private static JsonApiStateConverter GetState(this JsonSerializerOptions options)
        {
            return (JsonApiStateConverter) options.GetConverter(typeof(JsonApiStateConverter));
        }
    }
}
