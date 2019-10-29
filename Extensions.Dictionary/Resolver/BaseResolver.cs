using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Extensions.Dictionary.Resolver
{
    public abstract class BaseResolver : ISerializerResolver, IDisposable
    {
        protected const BindingFlags PublicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
        private static readonly IOptions<MemoryCacheOptions> CacheOptions = Options.Create(new MemoryCacheOptions());
        private bool disposed;

        protected MemoryCache MemberInfoCache { get; } = new MemoryCache(CacheOptions);

        /// <inheritdoc cref="ISerializerResolver" />
        public virtual string GetPropertyName(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            return memberInfo.Name;
        }

        /// <inheritdoc cref="ISerializerResolver" />
        public virtual object? GetPropertyValue(MemberInfo memberInfo, object? instance)
        {
            return memberInfo?.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetValue(instance),
                MemberTypes.Field => ((FieldInfo)memberInfo).GetValue(instance),
                null => throw new ArgumentNullException(nameof(memberInfo)),
                _ => throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.MemberType} not supported")
            };
        }

        /// <inheritdoc cref="ISerializerResolver" />
        public abstract IEnumerable<MemberInfo> GetMemberInfos(Type? type);

        /// <inheritdoc cref="IDisposable" />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    MemberInfoCache?.Dispose();
                }

                disposed = true;
            }
        }
    }
}
