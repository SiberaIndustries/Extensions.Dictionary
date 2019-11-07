using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensions.Dictionary.Resolver
{
    public class DefaultResolver : BaseResolver
    {
        public static readonly ISerializerResolver Instance = new DefaultResolver();

        /// <inheritdoc cref="BaseResolver" />
        public override IEnumerable<MemberInfo> GetMemberInfos(Type? type) => type == null
            ? Array.Empty<MemberInfo>()
            : MemberInfoCache.GetOrCreate(type.FullName, (entry) => type
                .GetProperties(PublicInstanceFlags).Cast<MemberInfo>()
                .Concat(type.GetFields(PublicInstanceFlags)));
    }
}
