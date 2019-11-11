using Extensions.Dictionary.Resolver;
using System;
using System.Reflection;

namespace Extensions.Dictionary
{
    internal static class MemberInfoExtensions
    {
        public static object? GetValue(this MemberInfo memberInfo, object? instance, ISerializerResolver serializerResolver) =>
            serializerResolver.GetMemberValue(memberInfo, instance);

        public static void SetValue(this MemberInfo memberInfo, object? instance, object? value)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(instance, value);
                    break;
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(instance, value);
                    break;
                case null:
                    throw new ArgumentNullException(nameof(memberInfo));
                default:
                    throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.DeclaringType.Name}.{memberInfo.Name} is not a property or field");
            }
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                FieldInfo fieldInfo => fieldInfo.FieldType,
                null => throw new ArgumentNullException(nameof(memberInfo)),
                _ => throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.DeclaringType.Name}.{memberInfo.Name} is not a property or field")
            };
        }

        public static string GetName(this MemberInfo memberInfo, ISerializerResolver serializerResolver) =>
            serializerResolver.GetMemberName(memberInfo);

        public static bool IsSimpleType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType.IsSimpleType(),
                FieldInfo fieldInfo => fieldInfo.FieldType.IsSimpleType(),
                null => throw new ArgumentNullException(nameof(memberInfo)),
                _ => throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.DeclaringType.Name}.{memberInfo.Name} is not a property or field")
            };
        }
    }
}
