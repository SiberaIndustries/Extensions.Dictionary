using Extensions.Dictionary.Resolver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Extensions.Dictionary.Tests
{
    public class JsonNetSerializerResolver : ISerializerResolver
    {
        public string GetPropertyName(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            return memberInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? memberInfo.Name;
        }

        public object? GetPropertyValue(MemberInfo memberInfo, object? instance)
        {
            var convertertype = memberInfo.GetCustomAttribute<JsonConverterAttribute>();
            if (convertertype != null)
            {
                var converter = (JsonConverter)Activator.CreateInstance(convertertype.ConverterType);
                using var sw = new StringWriter();
                using var jw = new JsonTextWriter(sw);
                converter.WriteJson(jw, memberInfo.GetValue(instance, this), new JsonSerializer());
                return sw.ToString().TrimStart('"').TrimEnd('"');
            }

            return memberInfo?.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetValue(instance),
                MemberTypes.Field => ((FieldInfo)memberInfo).GetValue(instance),
                null => throw new ArgumentNullException(nameof(memberInfo)),
                _ => throw new NotSupportedException($"{nameof(memberInfo.MemberType)} {memberInfo.MemberType} not supported")
            };
        }

        public IEnumerable<MemberInfo> GetMemberInfos(Type? type) =>
            type?
                .GetProperties(BindingFlags.Instance | BindingFlags.Public).Cast<MemberInfo>()
                .Concat(type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                .Where(x => !x.GetCustomAttributes<JsonIgnoreAttribute>().Any()) ?? Array.Empty<Type>();
    }
}
