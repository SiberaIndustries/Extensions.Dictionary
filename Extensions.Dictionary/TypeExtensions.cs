using System;
using System.Collections.Generic;

namespace Extensions.Dictionary
{
    internal static class TypeExtensions
    {
        private static readonly ISet<Type> SimpleTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(IDictionary<string, object>)
        };

        public static bool IsSimpleType(this Type? type) =>
            type == null || type.IsPrimitive || SimpleTypes.Contains(type);
    }
}
