﻿using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithAuthor
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("author")]
        public Author Author { get; set; }

        [JsonPropertyName("comments")]
        public Comment[] Comments { get; set; }
    }
}
