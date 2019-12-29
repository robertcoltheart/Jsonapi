using System;
using System.Linq;
using System.Reflection;

namespace Jsonapi.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsResource(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return properties.Any(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                       x.Name.Equals("Type", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
