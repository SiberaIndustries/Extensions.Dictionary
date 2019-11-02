using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public override string GetPropertyName(MemberInfo memberInfo) => memberInfo == null
            ? throw new ArgumentNullException(nameof(memberInfo))
            : MemberInfoCache.GetOrCreate(memberInfo.DeclaringType.FullName + '.' + memberInfo.Name, (entry) =>
            {
                var attribute = memberInfo.GetCustomAttribute(DataMemberAttrType, InspectAncestors);
                if (attribute != null)
                {
                    return ((DataMemberAttribute)attribute).Name ?? memberInfo.Name;
                }

                return memberInfo.Name;
            });

        /// <inheritdoc cref="BaseResolver" />
        public override IEnumerable<MemberInfo> GetMemberInfos(Type? type) => type == null
            ? Array.Empty<MemberInfo>()
            : MemberInfoCache.GetOrCreate(type.FullName, (entry) => type
                .GetProperties(PublicInstanceFlags).Cast<MemberInfo>()
                .Concat(type.GetFields(PublicInstanceFlags))
                .Where(x => x.GetCustomAttribute(IgnoreDataMemberAttrType, InspectAncestors) == null));
    }
}
