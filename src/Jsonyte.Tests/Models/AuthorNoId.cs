﻿using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class AuthorNoId
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
