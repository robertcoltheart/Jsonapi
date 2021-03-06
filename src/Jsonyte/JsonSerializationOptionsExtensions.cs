﻿using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;
using Jsonyte.Serialization.Reflection;

namespace Jsonyte
{
    /// <summary>
    /// Contains the <see cref="JsonSerializerOptions"/> extension methods.
    /// </summary>
    public static class JsonSerializationOptionsExtensions
    {
        /// <summary>
        /// Add JSON:API converters and capabilities to the specified <see cref="JsonSerializerOptions"/> instance.
        /// </summary>
        /// <remarks>
        /// In addition to adding the converters required for serializing and deserializing JSON:API data, the default
        /// <see cref="JsonSerializerOptions.PropertyNamingPolicy"/> value is set by default to be <see cref="JsonNamingPolicy.CamelCase"/>.
        /// This is only set, however, if there is no naming policy already set.
        ///
        /// To disable this behavior, simply set your naming policy prior to calling <see cref="AddJsonApi"/>.
        /// </remarks>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> instance.</param>
        /// <returns>The <see cref="JsonSerializerOptions"/> instance with JSON:API capabilities.</returns>
        public static JsonSerializerOptions AddJsonApi(this JsonSerializerOptions options)
        {
            if (!options.Converters.OfType<JsonApiStateConverter>().Any())
            {
                options.Converters.Add(new JsonApiStateConverter());
            }

            if (!options.Converters.OfType<JsonApiConverterFactory>().Any())
            {
                options.Converters.Add(new JsonApiConverterFactory());
            }

            if (!options.Converters.OfType<JsonApiResourceConverterFactory>().Any())
            {
                options.Converters.Add(new JsonApiResourceConverterFactory());
            }

            if (!options.Converters.OfType<JsonApiRelationshipConverterFactory>().Any())
            {
                options.Converters.Add(new JsonApiRelationshipConverterFactory());
            }

            options.PropertyNamingPolicy ??= JsonNamingPolicy.CamelCase;

            return options;
        }

        internal static StringComparer GetPropertyComparer(this JsonSerializerOptions options)
        {
            return options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;
        }

        internal static WrappedJsonConverter<T> GetWrappedConverter<T>(this JsonSerializerOptions options)
        {
            var converter = options.GetConverter(typeof(T));

            if (converter is not WrappedJsonConverter<T> jsonApiConverter)
            {
                throw new JsonApiException($"Converter not found for type {typeof(T)}");
            }

            return jsonApiConverter;
        }

        internal static JsonApiRelationshipDetailsConverter<T> GetRelationshipConverter<T>(this JsonSerializerOptions options)
        {
            var converter = options.GetConverter(typeof(RelationshipResource<T>));

            if (converter is not JsonApiRelationshipDetailsConverter<T> jsonApiConverter)
            {
                throw new JsonApiException($"Converter not found for type {typeof(T)}");
            }

            return jsonApiConverter;
        }

        internal static AnonymousRelationshipConverter GetAnonymousRelationshipConverter(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).AnonymousConverters.GetOrAdd(type, x =>
            {
                var converterType = typeof(AnonymousRelationshipConverter<>).MakeGenericType(x);

                var converter = Activator.CreateInstance(converterType, options);

                if (converter is not AnonymousRelationshipConverter relationshipConverter)
                {
                    throw new JsonApiException($"Cannot create JSON:API relationship converter: {type}");
                }

                return relationshipConverter;
            });
        }

        internal static JsonObjectConverter GetObjectConverter<T>(this JsonSerializerOptions options)
        {
            return GetState(options).ObjectConverters.GetOrAdd(typeof(T), _ => new JsonObjectConverter<T>(options.GetWrappedConverter<T>()));
        }

        internal static JsonObjectConverter GetObjectConverter(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).ObjectConverters.GetOrAdd(type, x =>
            {
                var converterType = typeof(JsonObjectConverter<>).MakeGenericType(x);
                var converter = options.GetConverter(type);

                var objectConverter = Activator.CreateInstance(converterType, converter);

                if (objectConverter is not JsonObjectConverter jsonObjectConverter)
                {
                    throw new JsonApiException($"Cannot create JSON:API converter: {type}");
                }

                return jsonObjectConverter;
            });
        }

        internal static JsonConverter<T> GetConverter<T>(this JsonSerializerOptions options)
        {
            return (JsonConverter<T>) options.GetConverter(typeof(T));
        }

        internal static JsonTypeInfo GetTypeInfo(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).GetTypeInfo(type, options);
        }

        internal static MemberAccessor GetMemberAccessor(this JsonSerializerOptions options)
        {
            return GetState(options).MemberAccessor;
        }

        private static JsonApiStateConverter GetState(JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(JsonApiStateConverter)) is not JsonApiStateConverter state)
            {
                throw new JsonApiException("JSON:API extensions not initialized, please use 'AddJsonApi' on 'JsonSerializerOptions' first");
            }

            return state;
        }
    }
}
