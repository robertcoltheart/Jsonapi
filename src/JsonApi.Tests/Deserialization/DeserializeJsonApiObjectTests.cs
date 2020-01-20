using System;
using System.Text.Json.Serialization;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeJsonApiObjectTests : JsonSerializerTests
    {
        [Fact]
        public void CanConvertJsonApiVersion()
        {
            const string json = @"
                {
                  'jsonapi': {
                    'version': '1.0'
                  }
                }";

            var document = Deserialize<Document>(json.ToDoubleQuoted());

            Assert.NotNull(document.JsonApi);
            Assert.Equal(document.JsonApi.Version, Version.Parse("1.0"));
        }

        [Fact]
        public void CanConvertNewJsonApiVersion()
        {
            const string json = @"
                {
                  'jsonapi': {
                    'version': '1.1'
                  }
                }";

            var document = Deserialize<Document>(json.ToDoubleQuoted());

            Assert.NotNull(document.JsonApi);
            Assert.Equal(document.JsonApi.Version, Version.Parse("1.1"));
        }

        [Fact]
        public void InvalidVersionThrows()
        {
            const string json = @"
                {
                  'jsonapi': {
                    'version': '1.b'
                  }
                }";

            var exception = Record.Exception(() => Deserialize<Document>(json.ToDoubleQuoted()));

            Assert.IsType<JsonApiException>(exception);
        }

        [Fact]
        public void LessThanMinimumVersionThrows()
        {
            const string json = @"
                {
                  'jsonapi': {
                    'version': '0.9'
                  }
                }";

            var exception = Record.Exception(() => Deserialize<Document>(json.ToDoubleQuoted()));

            Assert.IsType<JsonApiException>(exception);
        }

        private class Document
        {
            [JsonPropertyName("jsonapi")]
            public JsonApiObject JsonApi { get; set; }
        }
    }
}
