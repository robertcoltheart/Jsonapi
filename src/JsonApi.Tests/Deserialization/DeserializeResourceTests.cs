﻿using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeResourceTests : ValidationTests
    {
        [Fact]
        public void CanDeserializeSimpleObject()
        {
            const string json = @"
            {
              'data': {
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'Jsonapi'
                }
              }
            }";

            var article = json.Deserialize<Article>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }

        [Fact]
        public void CanDeserializeNestedObject()
        {
            const string json = @"
            {
              'data': {
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'Jsonapi',
                  'author': {
                    'name': 'Brown Smith',
                    'title': 'Mr'
                  }
                }
              }
            }";

            var article = json.Deserialize<ArticleWithNestedAuthor>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.NotNull(article.Author);
            Assert.Equal("Brown Smith", article.Author.Name);
            Assert.Equal("Mr", article.Author.Title);
        }

        [Fact]
        public void CanDeserializeSimpleArray()
        {
            const string json = @"
            {
              'data': [{
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'Jsonapi'
                }
              },
              {
                'type': 'articles',
                'id': '2',
                'attributes': {
                  'title': 'Jsonapi 2'
                }
              }]
            }";

            var articles = json.Deserialize<Article[]>();

            Assert.NotNull(articles);
            Assert.NotEmpty(articles);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal("Jsonapi", articles[0].Title);

            Assert.Equal("2", articles[1].Id);
            Assert.Equal("articles", articles[1].Type);
            Assert.Equal("Jsonap 2i", articles[1].Title);
        }
    }
}
