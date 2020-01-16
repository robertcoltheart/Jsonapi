using System.Text.Json;

namespace JsonApi.Tests
{
    public abstract class JsonSerializerTests
    {
        protected T Deserialize<T>(string json, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(json, options ?? GetJsonApiOptions());
        }

        private JsonSerializerOptions GetJsonApiOptions()
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            return options;
        }
    }
}
