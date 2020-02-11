using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeErrorTests
    {
        private const string SingleErrorJson = @"
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

        private const string MultipleErrorsJson = @"
            {
              'errors': [
                {
                  'id': '1',
                  'links': {
                    'about': 'http://example.com'
                  },
                  'status': '404',
                  'code': '123',
                  'title': 'Value is too short',
                  'detail': 'First name must contain at least three characters.',
                  'source': {
                    'pointer': '/data/attributes/firstName',
                    'parameter': 'id'
                  },
                  'meta': {
                    'copyright': 'jsonapi',
                    'authors': [
                      'Bob Jane',
                      'James Bond'
                    ]
                  }
                },
                {
                  'id': '2',
                  'links': {
                    'about': {
                      'href': 'http://example.com',
                      'meta': {
                        'count': 10,
                        'messages': [
                          'error 1',
                          'error 2'
                        ]
                      }
                    }
                  },
                  'status': '501',
                  'code': '456',
                  'title': 'No permission',
                  'detail': 'No permission to access the first name',
                  'source': {
                    'pointer': '/data/attributes/firstName',
                    'parameter': 'id'
                  },
                  'meta': {
                    'copyright': 'jsonapi',
                    'authors': [
                      'Bob Jane',
                      'James Bond'
                    ]
                  }
                },
                {
                  'code': '226',
                  'source': { 'pointer': '' },
                  'title': 'Password and password confirmation do not match.'
                }
              ]
            }";

        private const string MetaOnlyError = @"
            {
              'errors': [
                {
                  'meta': {
                    'copyright': 'jsonapi',
                    'authors': [
                      'Bob Jane',
                      'James Bond'
                    ]
                  }
                }
              ]
            }";

        private const string ErrorWithDocumentMeta = @"
            {
              'meta': {
                'name': 'Dave',
                'nemesis': [
                  'HAL',
                  'Space'
                ]
              },
              'errors': [
                {
                  'title': 'No permission',
                  'meta': {
                    'copyright': 'jsonapi',
                    'authors': [
                      'Bob Jane',
                      'James Bond'
                    ]
                  }
                }
              ]
            }";

        private const string NoErrorsDocument = @"
            {
              'data': {
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'json'
                }
              }
            }";

        private const string NoErrorsMultipleResourcesDocument = @"
            {
              'data': [
                {
                  'type': 'articles',
                  'id': '1',
                  'attributes': {
                    'title': 'json'
                  }
                },
                {
                  'type': 'articles',
                  'id': '2',
                  'attributes': {
                    'title': 'api'
                  }
                }
              ]
            }";

        [Fact]
        public void CanConvertSingleErrorAsArray()
        {
            var errors = SingleErrorJson.Deserialize<JsonApiError[]>();

            Assert.Single(errors);
            Assert.Equal("422", errors.First().Status);
            Assert.Equal("Invalid Attribute", errors.First().Title);
            Assert.Equal("First name must contain at least three characters.", errors.First().Detail);
            Assert.NotNull(errors.First().Source);
            Assert.Equal("/data/attributes/firstName", errors.First().Source.Pointer.ToString());
        }

        [Fact]
        public void CanConvertMultipleErrorsAsArray()
        {
            var errors = MultipleErrorsJson.Deserialize<JsonApiError[]>();

            Assert.NotNull(errors);
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void CanConvertMetaOnlyErrors()
        {
            var errors = MetaOnlyError.Deserialize<JsonApiError[]>();

            Assert.NotEmpty(errors);
            Assert.NotEmpty(errors.First().Meta);
        }

        [Fact]
        public void CanConvertErrorsWithDocumentMeta()
        {
            var errors = ErrorWithDocumentMeta.Deserialize<JsonApiError[]>();

            Assert.NotEmpty(errors);
            Assert.NotEmpty(errors.First().Meta);
            Assert.Equal("No permission", errors.First().Title);
        }

        [Theory]
        [InlineData(typeof(List<JsonApiError>))]
        [InlineData(typeof(JsonApiError[]))]
        [InlineData(typeof(IList<JsonApiError>))]
        [InlineData(typeof(ICollection<JsonApiError>))]
        [InlineData(typeof(IEnumerable<JsonApiError>))]
        public void CanConvertMultipleErrorsAsCollections(Type type)
        {
            var collection = MultipleErrorsJson.Deserialize(type);
            var enumerable = collection as IEnumerable<JsonApiError>;

            Assert.Equal(3, enumerable?.Count());
        }

        [Fact]
        public void SingleResourceDocumentWithNoErrorsReturnsNull()
        {
            var errors = NoErrorsDocument.Deserialize<JsonApiError[]>();

            Assert.Null(errors);
        }

        [Fact]
        public void MultipleResourceDocumentWithNoErrorsReturnsNull()
        {
            var errors = NoErrorsMultipleResourcesDocument.Deserialize<JsonApiError[]>();

            Assert.Null(errors);
        }
    }
}
