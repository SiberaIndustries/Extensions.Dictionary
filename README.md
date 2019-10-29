# Extensions.Dictionary

[![Build status](https://ci.appveyor.com/api/projects/status/o8eyfg4065t5qii9/branch/master?svg=true)](https://ci.appveyor.com/project/SiberaIndustries/extensions-dictionary/branch/master)
[![CodeFactor](https://www.codefactor.io/repository/github/siberaindustries/extensions.dictionary/badge)](https://www.codefactor.io/repository/github/siberaindustries/extensions.dictionary)
[![codecov](https://codecov.io/gh/SiberaIndustries/Extensions.Dictionary/branch/master/graph/badge.svg)](https://codecov.io/gh/SiberaIndustries/Extensions.Dictionary)

## Introduction

The easiest way to start using `Extensions.Dictionary` is to install it’s Nuget package. In Visual Studio open Package Manager console and type the following:

```cs
Install-Package Extensions.Dictionary
```

In the source file where you will be using Dictionary Extensions import the namespace:

```cs
using Extensions.Dictionary;
```

Convert an instance to a dictionary:

```cs
Person person = ..;

// Option 1: Convert but ignore DataContract attributes
var dictionary1 = person.ToDictionary();

// Option 2: Convert and respect DataContract attributes
var dictionary2 = person.ToDictionary(new DataContractResolver());
```

Convert a dictionary back to it's typed instance:

```cs
var person = dictionary.ToInstance<Person>();
```

## Extensibility

It is possible to customize the member name / value extraction of objects. Just implement the provided `ISerializerResolver` interface. The following code snipped shows an example of a [Json.NET](https://www.newtonsoft.com/json) resolver:

```cs
public class JsonNetResolver : ISerializerResolver
{
    public string GetPropertyName(MemberInfo memberInfo)
    {
        if (memberInfo == null)
        {
            throw new ArgumentNullException(nameof(memberInfo));
        }

        var attribute = memberInfo.GetCustomAttribute<JsonPropertyAttribute>();
        return attribute?.PropertyName ?? memberInfo.Name;
    }

    public object GetPropertyValue(MemberInfo memberInfo, object instance)
    {
        switch (memberInfo?.MemberType)
        {
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).GetValue(instance);
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).GetValue(instance);
            case null:
                throw new ArgumentNullException(nameof(memberInfo));
            default:
                throw new NotSupportedException(memberInfo.MemberType + " not supported.");
        }
    }

    public IEnumerable<MemberInfo> GetMemberInfos(Type type)
    {
        return type?
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Cast<MemberInfo>()
            .Concat(type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            .Where(x => !x.GetCustomAttributes<JsonIgnoreAttribute>().Any())
            ?? Array.Empty<Type>();
    }
}
```

## Third party licenses

- Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
