using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;

namespace Extensions.Dictionary.Resolver
{
    public class DefaultResolver : BaseResolver
    {
        public static readonly ISerializerResolver Instance = new DefaultResolver();

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

            return MemberInfoCache.Set(key, type.GetPropertiesAndFields());
        }
    }
}
