﻿using System.Text.Json;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Reflection
{
    public class ConstructorAttributeTests
    {
        [Fact(Skip = "Disabled until constructor support is better")]
        public void ResourceWithMultipleMarkedConstructorsThrows()
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

            var exception = Record.Exception(() => json.Deserialize<ArticleWithMultipleConstructors>());

            Assert.IsType<JsonException>(exception);
            Assert.Contains("Cannot have multiple constructors marked", exception.Message);
        }

        [Fact(Skip = "Disabled until constructor support is better")]
        public void CanDeserializeResourceWithMarkedConstructor()
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

            var article = json.Deserialize<ArticleWithConstructor>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }

        [Fact(Skip = "Disabled until constructor support is better")]
        public void CanDeserializeWithMissingConstructorParametersAndWritableProperty()
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

            var article = json.Deserialize<ArticleWithConstructorMissingParameter>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }

        [Fact(Skip = "Disabled until constructor support is better")]
        public void CanDeserializeWithMissingConstructorParametersAndReadOnlyProperty()
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

            var article = json.Deserialize<ArticleWithConstructorMissingParameterReadOnly>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Null(article.Title);
        }
    }
}
