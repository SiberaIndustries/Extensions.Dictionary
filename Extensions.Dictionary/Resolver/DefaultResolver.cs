﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensions.Dictionary.Resolver
{
    public class DefaultResolver : ISerializerResolver
    {
        /// <inheritdoc cref="ISerializerResolver" />
        public string GetPropertyName(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            return memberInfo.Name;
        }

        /// <inheritdoc cref="ISerializerResolver" />
        public object? GetPropertyValue(MemberInfo memberInfo, object? instance)
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
        public IEnumerable<MemberInfo> GetMemberInfos(Type? type) =>
            type?
                .GetProperties(BindingFlags.Instance | BindingFlags.Public).Cast<MemberInfo>()
                .Concat(type.GetFields(BindingFlags.Instance | BindingFlags.Public)) ?? Array.Empty<Type>();
    }
}
