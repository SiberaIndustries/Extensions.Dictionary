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
        private static readonly Type CustomAttributeType = typeof(DataMemberAttribute);

        public bool InspectAncestors { get; set; } = true;

        /// <inheritdoc cref="ISerializerResolver" />
        public override string GetPropertyName(MemberInfo memberInfo) => memberInfo == null
            ? throw new ArgumentNullException(nameof(memberInfo))
            : MemberInfoCache.GetOrCreate(memberInfo.Name, (entry) =>
            {
                var attribute = memberInfo.GetCustomAttribute(CustomAttributeType, InspectAncestors);
                if (attribute != null)
                {
                    return ((DataMemberAttribute)attribute).Name;
                }

                return base.GetPropertyName(memberInfo);
            });

        /// <inheritdoc cref="ISerializerResolver" />
        public override IEnumerable<MemberInfo> GetMemberInfos(Type? type) => type == null
            ? Array.Empty<MemberInfo>()
            : MemberInfoCache.GetOrCreate(type, (entry) => type
                .GetProperties(PublicInstanceFlags).Cast<MemberInfo>()
                .Concat(type.GetFields(PublicInstanceFlags))
                .Where(x => x.GetCustomAttribute(CustomAttributeType, InspectAncestors) == null));
    }
}
