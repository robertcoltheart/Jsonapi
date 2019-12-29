using JsonApiSerializer;
using Newtonsoft.Json;
using Xunit;

namespace Jsonapi.Tests
{
    public class JsonApiSerializerTests
    {
        [Fact]
        public void Test()
        {
            var settings = new JsonApiSerializerSettings();
            settings.Formatting = Formatting.Indented;

            var article = new Article
            {
                Id = "1",
                Title = "title",
                Location = new Location
                {
                    Country = "SG",
                    Tags = new[]
                    {
                        new Tag {Name = "tag1"}
                    }
                },
                Tags = new[]
                {
                    new Tag {Name = "tag2"},
                    new Tag {Name = "tag3"}
                }
            };

            var json = JsonConvert.SerializeObject(article, settings);
        }

        public class Article
        {
            public string Id { get; set; }

            public string Title { get; set; }

            public Tag[] Tags { get; set; }

            public Location Location { get; set; }
        }

        public class Tag
        {
            public string Name { get; set; }
        }

        public class Location
        {
            public string Country { get; set; }

            public Tag[] Tags { get; set; }
        }
    }
}
