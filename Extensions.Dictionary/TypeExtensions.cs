using System;
using System.Collections.Generic;
using System.Reflection;

namespace Extensions.Dictionary
{
    internal static class TypeExtensions
    {
        private const BindingFlags PublicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
        private static readonly Type StringType = typeof(string);

        public static bool IsSimpleType(this Type? type) =>
            type == null || type.IsPrimitive || type == StringType;

        public static MemberInfo[] GetPropertiesAndFields(this Type type)
        {
            var props = (MemberInfo[])type.GetProperties(PublicInstanceFlags);
            var fields = (MemberInfo[])type.GetFields(PublicInstanceFlags);

            var oldLength = props.Length;
            Array.Resize(ref props, props.Length + fields.Length);
            fields.CopyTo(props, oldLength);
            return props;
        }

        public static MemberInfo[] GetPropertiesAndFieldsFiltered(this Type type, Type attributeType, bool inspectAncestors = false)
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

            return list.ToArray();
        }
    }
}
