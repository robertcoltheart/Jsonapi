using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeResourceTests : ValidationTests
    {
        private const string SimpleArticle = @"
            {
              'data': {
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'Jsonapi'
                }
              }
            }";

        [Fact]
        public void CanDeserializeSimpleObject()
        {
            var article = SimpleArticle.Deserialize<Article>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }
    }
}
