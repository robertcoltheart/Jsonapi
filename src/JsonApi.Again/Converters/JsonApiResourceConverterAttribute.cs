using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Jsonapi.Converters
{
    public class JsonApiResourceConverterAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            var resourceType = typeToConvert.GetElementType().GenericTypeArguments.First();

            var type = typeof(JsonApiResourceConverter<>).MakeGenericType(typeToConvert);

            return Activator.CreateInstance(type) as JsonConverter;
        }
    }
}
