using System.Linq;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeErrorTests : JsonSerializerTests
    {
        [Fact]
        public void CanConvertSingleErrorAsArray()
        {
            const string json = @"
                {
                  'errors': [
                    {
                      'status': '422',
                      'source': { 'pointer': '/data/attributes/firstName' },
                      'title':  'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.'
                    }
                  ]
                }";

            var errors = Deserialize<JsonApiError[]>(json.ToDoubleQuoted());

            Assert.Single(errors);
            Assert.Equal("422", errors.First().Status);
            Assert.Equal("Invalid Attribute", errors.First().Title);
            Assert.Equal("First name must contain at least three characters.", errors.First().Detail);
            Assert.NotNull(errors.First().Source);
            Assert.Equal("/data/attributes/firstName", errors.First().Source.Pointer.ToString());
        }

        [Fact]
        public void CanConvertSingleError()
        {
            const string json = @"
                {
                  'errors': [
                    {
                      'status': '422',
                      'source': { 'pointer': '/data/attributes/firstName' },
                      'title':  'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.'
                    }
                  ]
                }";

            var error = Deserialize<JsonApiError>(json.ToDoubleQuoted());

            Assert.NotNull(error);
            Assert.Equal("422", error.Status);
            Assert.Equal("Invalid Attribute", error.Title);
            Assert.Equal("First name must contain at least three characters.", error.Detail);
            Assert.NotNull(error.Source);
            Assert.Equal("/data/attributes/firstName", error.Source.Pointer.ToString());
        }
    }
}
