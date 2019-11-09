using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;

namespace Extensions.Dictionary.Resolver
{
    public class DefaultResolver : BaseResolver
    {
        public static readonly ISerializerResolver Instance = new DefaultResolver();

        /// <inheritdoc cref="BaseResolver" />
        public override MemberInfo[] GetMemberInfos(Type? type) => type == null
            ? Array.Empty<MemberInfo>()
            : MemberInfoCache.GetOrCreate(type.FullName, (entry) => type.GetPropertiesAndFields());
    }
}
