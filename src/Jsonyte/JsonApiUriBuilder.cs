﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Text;

namespace Jsonyte
{
    /*
        TODO:


        include
        fields
        sort
        paging
        filtering


        =========================

        GET /articles/1?include=author,comments.author

        GET /articles?fields[articles]=title,body&fields[people]=name

        GET /people?sort=age
        GET /articles?sort=-created,title

        Paging (needs strategy)
        page[number]
        page[size]
        OR
        page[offset]
        page[limit]
        OR
        page[cursor]

        Filtering (needs strategy)
        filter[name]=something
     */
    public class JsonApiUriBuilder : UriBuilder
    {
        private static readonly ConcurrentDictionary<Type, string> TypeNames = new();

        private readonly List<string> includes = new();

        private readonly List<string> sorts = new();

        private readonly Dictionary<string, List<string>> includedFields = new();

        public JsonNamingPolicy NamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;

        public NameValueCollection GetQueryParameters()
        {
            return ParseQuery();
        }

        public JsonApiUriBuilder AddQuery(string key, params string[] values)
        {
            var parameters = ParseQuery();

            if (values.Length == 0)
            {
                AddQueryParameter(parameters, null, key);
            }
            else
            {
                foreach (var value in values)
                {
                    AddQueryParameter(parameters, key, value);
                }
            }

            Query = parameters.ToString();

            return this;
        }

        private void AddQueryParameter(NameValueCollection parameters, string? key, string value)
        {
            var existing = parameters.GetValues(key);

            if (existing == null || !existing.Contains(value))
            {
                parameters.Add(key, value);
            }
        }

        public JsonApiUriBuilder Include(string relationship)
        {
            var include = NamingPolicy.ConvertName(relationship);

            return AddQuery("include", include);
        }

        public JsonApiUriBuilder OrderBy(string member)
        {
            AddMember(sorts, NamingPolicy.ConvertName(member));

            return this;
        }

        public JsonApiUriBuilder OrderByDescending(string member)
        {
            AddMember(sorts, $"-{NamingPolicy.ConvertName(member)}");

            return this;
        }

        public JsonApiUriBuilder IncludeField(string type, string member)
        {
            return IncludeFields(type, member);
        }

        public JsonApiUriBuilder IncludeFields(string type, params string[] members)
        {
            if (includedFields.TryGetValue(type, out var values))
            {
                includedFields[type] = values = new List<string>();
            }

            foreach (var member in members)
            {
                AddMember(values, NamingPolicy.ConvertName(member));
            }

            return this;
        }

        internal static string GetTypeName(Type type, JsonNamingPolicy namingPolicy, string? name)
        {
            return TypeNames.GetOrAdd(type, x =>
            {
                // 1. Use name passed in first
                if (!string.IsNullOrEmpty(name))
                {
                    TypeNames[x] = name!;

                    return name!;
                }

                // 2. Try and create the object and use the Type property
                if (x.IsResource())
                {
                    var member = x.GetTypeMember();
                    var constructor = x.GetConstructor(Type.EmptyTypes);

                    if (constructor != null)
                    {
                        var resource = constructor.Invoke(null);

                        var value = member switch
                        {
                            PropertyInfo property => property.GetValue(resource)?.ToString(),
                            FieldInfo field => field.GetValue(resource)?.ToString(),
                            _ => null
                        };

                        if (!string.IsNullOrEmpty(value))
                        {
                            TypeNames[x] = value;

                            return value;
                        }
                    }
                }

                // 3. Use the type name and make a best guess
                var typeName = namingPolicy.ConvertName(x.Name);

                return Pluralizer.Pluralize(typeName);
            });
        }

        private void UpdateQuery()
        {
            var parameters = ParseQuery();

            if (includes.Any())
            {
                parameters.Add("include", string.Join(",", includes));
            }

            var values = parameters.AllKeys
                .Select(x => $"{Uri.EscapeDataString(x)}={Uri.EscapeDataString(parameters[x])}");

            Query = string.Join("&", values);
        }

        private NameValueCollection ParseQuery()
        {
            var result = new QueryParametersCollection();

            var query = Query;

            if (query.Length > 0 && query[0] == '?')
            {
                query = query.Substring(1);
            }

            var queryParts = query.Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var queryPart in queryParts)
            {
                var index = queryPart.IndexOf('=');

                if (index != -1)
                {
                    var name = queryPart.Substring(0, index);
                    var value = queryPart.Substring(index + 1, queryPart.Length - index - 1);

                    result.Add(name, value);
                }
                else
                {
                    result.Add(null, queryPart);
                }
            }

            return result;
        }

        private void AddMember(List<string> values, string value)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }
    }

    public class JsonApiUriBuilder<T> : JsonApiUriBuilder
    {
        private readonly List<string> sorts = new();

        private readonly Dictionary<string, List<string>> includedFields = new();

        private readonly Dictionary<string, List<string>> excludedFields = new();

        public JsonApiUriBuilder<T> Include<TRelationship>(Expression<Func<T, TRelationship>> expression)
        {
            Include(GetMember(expression));

            return this;
        }

        public JsonApiUriBuilder<T> OrderBy<TMember>(Expression<Func<T, TMember>> expression)
        {
            AddMember(sorts, GetMember(expression));

            return this;
        }

        public JsonApiUriBuilder<T> OrderByDescending<TMember>(Expression<Func<T, TMember>> expression)
        {
            AddMember(sorts, $"-{GetMember(expression)}");

            return this;
        }

        public JsonApiUriBuilder<T> IncludeAllFields()
        {
            return this;
        }

        public JsonApiUriBuilder<T> IncludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            AddIncludedField(expression, includedFields, type);

            return this;
        }

        public JsonApiUriBuilder<T> ExcludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            AddIncludedField(expression, excludedFields, type);

            return this;
        }

        private void AddMember(List<string> values, string value)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }

        private void AddIncludedField<TMember>(Expression<Func<T, TMember>> expression, Dictionary<string, List<string>> fields, string? type = null)
        {
            var member = GetFinalMember(expression);

            if (member == null)
            {
                throw new JsonApiException($"Cannot parse expression: {expression}");
            }

            var field = NamingPolicy.ConvertName(GetMemberName(member.Member));
            var typeName = GetTypeName(member.Member.DeclaringType!, NamingPolicy, type);

            if (!fields.TryGetValue(typeName, out var values))
            {
                fields[typeName] = values = new List<string>();
            }

            if (!values.Contains(field))
            {
                values.Add(field);
            }
        }

        private MemberExpression? GetFinalMember<TMember>(Expression<Func<T, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            while (memberExpression != null)
            {
                if (memberExpression.Expression is not MemberExpression nested)
                {
                    return memberExpression;
                }

                memberExpression = nested;
            }

            return null;
        }

        private string GetMember<TMember>(Expression<Func<T, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var values = new List<string>();

            while (memberExpression != null)
            {
                var name = GetMemberName(memberExpression.Member);

                values.Add(NamingPolicy.ConvertName(name));

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            values.Reverse();

            return string.Join(".", values);
        }

        private string GetMemberName(MemberInfo member)
        {
            var nameAttribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();

            return nameAttribute != null
                ? nameAttribute.Name
                : member.Name;
        }
    }
}
