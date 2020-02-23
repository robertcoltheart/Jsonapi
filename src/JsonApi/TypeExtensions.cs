using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonApi
{
    internal static class TypeExtensions
    {
        public static bool IsDocument(this Type type)
        {
            return type == typeof(JsonApiDocument) ||
                   type.IsGenericType && type.GetGenericTypeDefinition() == typeof(JsonApiDocument<>);
        }

        public static bool IsCollection(this Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static Type GetCollectionType(this Type type)
        {
            if (type == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            var genericType = GetInterfaces(type)
                .Where(x => x.IsGenericType)
                .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return genericType?.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
        }

        private static IEnumerable<Type> GetInterfaces(Type type)
        {
            yield return type;

            foreach (var typeInterface in type.GetInterfaces())
            {
                yield return typeInterface;
            }
        }
    }
}
