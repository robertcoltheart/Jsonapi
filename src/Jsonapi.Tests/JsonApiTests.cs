using System.Text.Json;
using Jsonapi.Converters;
using Xunit;

namespace Jsonapi.Tests
{
    public class JsonApiTests
    {
        [Fact]
        public void Syntax()
        {
            const string json = @"
{
  'type': 'articles',
  'id': '1',
  'attributes': {
    'title': 'Rails is Omakase'
  }
}";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = {new JsonApiConverterFactory()}
            };

            var item = JsonSerializer.Deserialize<Article>(json, options);
        }

        [Fact]
        public void CanConvertData()
        {
            const string json = @"
{
  'data': {
    'id': '1',
    'type': 'articles',
    'attributes': {
      'title': 'My article'
    },
    'relationships': {
      'author': {
        'data': {
          'id': '2',
          'type': 'authors'
        }
      }
    }
  },
  'included': [
    {
      'id': '2',
      'type': 'authors',
      'attributes': {
        'name': 'Rob'
      },
      'relationships': {
        'country': {
          'data': {
            'id': '3',
            'type': 'country'
          }
        }
      }
    },
    {
      'id': '3',
      'type': 'country',
      'attributes': {
        'zone': 'NZ'
      }
    }
  ]
}";
            var options = new JsonSerializerOptions
            {
                Converters = {new JsonApiConverterFactory()}
            };

            var article = JsonSerializer.Deserialize<Article>(json, options);
        }

        [Fact]
        public void CanWriteData()
        {
            var article = new Article
            {
                Id = 1,
                Title = "My article",
                Author = new Author
                {
                    Id = 2,
                    Name = "Rob",
                    Country = new Country
                    {
                        Id = 3,
                        Zone = "NZ"
                    }
                }
            };

            var json = JsonSerializer.Serialize(article);
        }

        private class Article
        {
            public int Id { get; set; }

            public string Type { get; } = "articles";

            public string Title { get; set; }

            public Author Author { get; set; }
        }

        private class Author
        {
            public int Id { get; set; }

            public string Type { get; } = "authors";

            public string Name { get; set; }

            public Country Country { get; set; }
        }

        private class Country
        {
            public int Id { get; set; }

            public string Type { get; } = "country";

            public string Zone { get; set; }
        }
    }
}
