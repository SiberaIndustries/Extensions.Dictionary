using Extensions.Dictionary.Resolver;
using System;
using System.Reflection;

namespace Extensions.Dictionary
{
    internal static class MemberInfoExtensions
    {
        public static object? GetValue(this MemberInfo memberInfo, object? instance, ISerializerResolver serializerResolver) =>
            serializerResolver.GetPropertyValue(memberInfo, instance);

        public static string GetName(this MemberInfo memberInfo, ISerializerResolver serializerResolver) =>
            serializerResolver.GetPropertyName(memberInfo);

        public static bool IsSimpleType(this MemberInfo memberInfo)
        {
            return memberInfo?.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)memberInfo).PropertyType.IsSimpleType(),
                MemberTypes.Field => ((FieldInfo)memberInfo).FieldType.IsSimpleType(),
                null => throw new ArgumentNullException(nameof(memberInfo)),
                _ => throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.MemberType} not supported")
            };
        }
    }
}
