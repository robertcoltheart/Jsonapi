﻿using System;
using System.Text.Json;
using AutoBogus;
using BenchmarkDotNet.Attributes;
using Bogus;
using JsonApi.Tests.Models;
using JsonApiSerializer;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonApi.Tests.Performance
{
    public class SerializeDeserializeBenchmarks
    {
        private readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly JsonApiSerializerSettings settings = new();

        public ManyTypesModel simpleModel;

        private Data<ManyTypesModel> simpleModelRaw;

        private ArticleWithAuthor baselineModel;

        private ArticleWithAuthor[] articles;

        [GlobalSetup]
        public void Setup()
        {
            options.AddJsonApi();

            Randomizer.Seed = new Random(56178921);
            simpleModel = AutoFaker.Generate<ManyTypesModel>();
            simpleModelRaw = new Data<ManyTypesModel>
            {
                Id = simpleModel.Id,
                Type = simpleModel.Type,
                Attributes = simpleModel
            };

            articles = AutoFaker.Generate<ArticleWithAuthor[]>(x => x.WithRepeatCount(20));

            var author = new Author
            {
                Id = "9",
                Type = "people",
                Name = "Dan Gebhardt",
                Twitter = "dgeb"
            };

            baselineModel = new ArticleWithAuthor
            {
                Id = "1",
                Type = "articles",
                Title = "JSON:API paints my bikeshed!",
                Author = author,
                Comments = new[]
                {
                    new Comment
                    {
                        Id = "5",
                        Type = "comments",
                        Body = "First!",
                        Author = author
                    },
                    new Comment
                    {
                        Id = "12",
                        Type = "comments",
                        Body = "I like XML better",
                        Author = author
                    }
                }
            };
        }

        [Benchmark]
        public string SerializeSimpleObject()
        {
            return JsonSerializer.Serialize(simpleModel, options);
        }

        [Benchmark]
        public string SerializeSimpleObjectRaw()
        {
            return JsonSerializer.Serialize(simpleModel);
        }

        //[Benchmark]
        //public string SerializeSimpleObjectWithNewtonsoft()
        //{
        //    return JsonConvert.SerializeObject(simpleModel, settings);
        //}

        //[Benchmark]
        //public string SerializeBaselineObject()
        //{
        //    return JsonSerializer.Serialize(baselineModel, options);
        //}

        //[Benchmark]
        //public string SerializeBaselineObjectRaw()
        //{
        //    return JsonSerializer.Serialize(baselineModel);
        //}

        //[Benchmark]
        //public string SerializeBaselineObjectWithNewtonsoft()
        //{
        //    return JsonConvert.SerializeObject(baselineModel, settings);
        //}

        //[Benchmark]
        //public string SerializeLargeCompoundCollection()
        //{
        //    return JsonSerializer.Serialize(articles, options);
        //}

        //[Benchmark]
        //public string SerializeLargeCompoundCollectionWithNewtonsoft()
        //{
        //    return JsonConvert.SerializeObject(articles, settings);
        //}

        //[Benchmark]
        //public string DeserializeSimpleObject()
        //{
        //    return string.Empty;
        //}

        //[Benchmark]
        //public string DeserializeSimpleObjectWithNewtonsoft()
        //{
        //    return string.Empty;
        //}

        //[Benchmark]
        //public string DeserializeBaselineObject()
        //{
        //    return string.Empty;
        //}

        //[Benchmark]
        //public string DeserializeBaselineObjectWithNewtonsoft()
        //{
        //    return string.Empty;
        //}

        //[Benchmark]
        //public string DeserializeLargeCompoundCollection()
        //{
        //    return string.Empty;
        //}

        //[Benchmark]
        //public string DeserializeLargeCompoundCollectionWithNewtonsoft()
        //{
        //    return string.Empty;
        //}
    }
}