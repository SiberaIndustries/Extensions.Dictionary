using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Extensions.Dictionary.Resolver
{
    public class DataContractResolver : BaseResolver
    {
        private static readonly Type DataMemberAttrType = typeof(DataMemberAttribute);
        private static readonly Type IgnoreDataMemberAttrType = typeof(IgnoreDataMemberAttribute);

        /// <summary>
        /// Gets or sets a value indicating whether to inspect the ancestors of element or not.
        /// </summary>
        public bool InspectAncestors { get; set; } = true;

        /// <inheritdoc cref="BaseResolver" />
        public override string GetMemberName(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            var key = memberInfo.DeclaringType.FullName + '.' + memberInfo.Name;
            if (MemberInfoCache.TryGetValue(key, out string value))
            {
                return value;
            }

            var attribute = memberInfo.GetCustomAttribute(DataMemberAttrType, InspectAncestors);
            value = attribute != null
                ? ((DataMemberAttribute)attribute).Name ?? memberInfo.Name
                : memberInfo.Name;
            return MemberInfoCache.Set(key, value);
        }

        /// <inheritdoc cref="BaseResolver" />
        public override MemberInfo[] GetMemberInfos(Type? type)
        {
            if (type == null)
            {
                return Array.Empty<MemberInfo>();
            }

            var key = type.FullName;
            if (MemberInfoCache.TryGetValue(key, out MemberInfo[] value))
            {
                return value;
            }

            return MemberInfoCache.Set(key, type.GetPropertiesAndFieldsFiltered(IgnoreDataMemberAttrType, InspectAncestors));
        }
    }
}
