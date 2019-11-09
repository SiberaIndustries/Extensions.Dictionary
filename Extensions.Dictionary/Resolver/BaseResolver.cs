﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace Extensions.Dictionary.Resolver
{
    public abstract class BaseResolver : ISerializerResolver, IDisposable
    {
        private static readonly IOptions<MemoryCacheOptions> CacheOptions = Options.Create(new MemoryCacheOptions());
        private bool disposed;

        protected MemoryCache MemberInfoCache { get; } = new MemoryCache(CacheOptions);

        /// <inheritdoc cref="ISerializerResolver" />
        public virtual string GetMemberName(MemberInfo memberInfo) => memberInfo == null
            ? throw new ArgumentNullException(nameof(memberInfo))
            : memberInfo.Name;

        /// <inheritdoc cref="ISerializerResolver" />
        public virtual object? GetMemberValue(MemberInfo memberInfo, object? instance)
        {
            return memberInfo?.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetValue(instance),
                MemberTypes.Field => ((FieldInfo)memberInfo).GetValue(instance),
                null => throw new ArgumentNullException(nameof(memberInfo)),
                _ => throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.DeclaringType.Name}.{memberInfo.Name} is not a property or field")
            };
        }

        /// <inheritdoc cref="ISerializerResolver" />
        public abstract MemberInfo[] GetMemberInfos(Type? type);

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
