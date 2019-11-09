using Extensions.Dictionary.Resolver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Extensions.Dictionary
{
    internal static class MemberInfoExtensions
    {
        private const BindingFlags PublicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;

        internal static MemberInfo[] GetPropertiesAndFields(this Type type)
        {
            var props = (MemberInfo[])type.GetProperties(PublicInstanceFlags);
            var fields = (MemberInfo[])type.GetFields(PublicInstanceFlags);

            var oldLength = props.Length;
            Array.Resize(ref props, props.Length + fields.Length);
            fields.CopyTo(props, oldLength);
            return props;
        }

        internal static MemberInfo[] GetPropertiesAndFieldsFiltered(this Type type, Type attributeType, bool inspectAncestors = false)
        {
            var props = (MemberInfo[])type.GetProperties(PublicInstanceFlags);
            var fields = (MemberInfo[])type.GetFields(PublicInstanceFlags);

            var max = props.Length > fields.Length ? props.Length : fields.Length;
            var list = new List<MemberInfo>(props.Length + fields.Length);
            for (int i = 0; i < max; i++)
            {
                if (i < props.Length && props[i].GetCustomAttribute(attributeType, inspectAncestors) == null)
                {
                    list.Add(props[i]);
                }

                if (i < fields.Length && fields[i].GetCustomAttribute(attributeType, inspectAncestors) == null)
                {
                    list.Add(fields[i]);
                }
            }

            var array = new MemberInfo[list.Count];
            list.CopyTo(array);
            return array;
        }

        public static object? GetValue(this MemberInfo memberInfo, object? instance, ISerializerResolver serializerResolver) =>
            serializerResolver.GetPropertyValue(memberInfo, instance);

        public static void SetValue(this MemberInfo memberInfo, object? instance, object? value)
        {
            switch (memberInfo?.MemberType)
            {
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(instance, value);
                    break;
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(instance, value);
                    break;
                case null:
                    throw new ArgumentNullException(nameof(memberInfo));
                default:
                    throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.MemberType} not supported");
            }
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo?.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)memberInfo).PropertyType,
                MemberTypes.Field => ((FieldInfo)memberInfo).FieldType,
                null => throw new ArgumentNullException(nameof(memberInfo)),
                _ => throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.MemberType} not supported")
            };
        }

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
