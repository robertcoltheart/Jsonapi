using System.Text.Json;
using Jsonapi.Converters;
using Jsonapi.Tests.Models;
using Xunit;

namespace Jsonapi.Tests
{
    public class Tests
    {
        [Fact]
        public void Test()
        {
            const string json = @"
{
  ""data"": {
    ""type"": ""articles"",
    ""id"": ""1"",
    ""attributes"": {
      ""title"": ""Rails is Omakase""
    }
  }
}";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = {new JsonApiConverterFactory()}
            };

            var article = JsonSerializer.Deserialize<Article>(json, options);
        }
    }
}
