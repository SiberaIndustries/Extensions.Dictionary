using System.Reflection;

namespace Extensions.Dictionary.Internal
{
    internal static class TypeExtensions
    {
        public static MemberInfo[] GetPropertiesAndFields(this Type type)
        {
            var (props, fields) = type.GetPropertiesAndFieldsInternal();
            if (fields.Length > 0)
            {
                var oldLength = props.Length;
                Array.Resize(ref props, props.Length + fields.Length);
                fields.CopyTo(props, oldLength);
            }

            return props;
        }

        public static MemberInfo[] GetPropertiesAndFieldsFiltered(this Type type, Type attributeType, bool inspectAncestors = false)
        {
            var (props, fields) = type.GetPropertiesAndFieldsInternal();
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

        internal static (MemberInfo[] props, MemberInfo[] fields) GetPropertiesAndFieldsInternal(this Type type)
        {
            const BindingFlags PublicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
            var props = type.GetProperties(PublicInstanceFlags).Where(x => !(x.Name == "Item" && x.GetIndexParameters().Length == 1)).ToArray();
            var fields = type.GetFields(PublicInstanceFlags);
            return (props, fields);
        }
    }
}
