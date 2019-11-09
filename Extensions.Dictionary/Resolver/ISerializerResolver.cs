using System;
using System.Reflection;

namespace Extensions.Dictionary.Resolver
{
    /// <summary>
    /// Represents an interface of how to resolve member information of an object.
    /// </summary>
    public interface ISerializerResolver
    {
        /// <summary>
        /// Returns the name of the property.
        /// </summary>
        /// <param name="memberInfo">MemberInfo object.</param>
        /// <returns>The name of the propety.</returns>
        public string GetPropertyName(MemberInfo memberInfo);

        /// <summary>
        /// Returns the value of the property.
        /// </summary>
        /// <param name="memberInfo">MemberInfo object.</param>
        /// <param name="instance">The instance of the object.</param>
        /// <returns>The value of the propety.</returns>
        public object? GetPropertyValue(MemberInfo memberInfo, object? instance);

        /// <summary>
        /// Returns the necessary MemberInfo objects.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>The necessary MemberInfo objects.</returns>
        public MemberInfo[] GetMemberInfos(Type? type);
    }
}
