# Extensions.Dictionary

[![NuGet](https://img.shields.io/nuget/v/Extensions.Dictionary.svg)](https://www.nuget.org/packages/Extensions.Dictionary)
[![Build status](https://ci.appveyor.com/api/projects/status/o8eyfg4065t5qii9/branch/master?svg=true)](https://ci.appveyor.com/project/SiberaIndustries/extensions-dictionary/branch/master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_Extensions.Dictionary&metric=alert_status)](https://sonarcloud.io/dashboard?id=SiberaIndustries_Extensions.Dictionary)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_Extensions.Dictionary&metric=coverage)](https://sonarcloud.io/dashboard?id=SiberaIndustries_Extensions.Dictionary)
[![CodeFactor](https://www.codefactor.io/repository/github/siberaindustries/extensions.dictionary/badge)](https://www.codefactor.io/repository/github/siberaindustries/extensions.dictionary)

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

## Benchmarks

The following benchmarks are showing the differences between converting with reflections (DefaultResolver) and various JSON serialization frameworks. SPOILER: You get the best efficiency with reflections.

**Test environment**

- BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
- Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 8 physical cores
- .NET Core SDK=3.0.100

### Convert an instance to a dictionary

|                              Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------:|----------:|----------:|----------:|------:|--------:|-----:|-------:|------:|------:|----------:|
|                     DefaultResolver |  8.184 us | 0.2507 us | 0.7235 us |  7.930 us |  1.00 |    0.00 |    1 | 0.8087 |     - |     - |   2.51 KB |
|                DataContractResolver | 42.801 us | 1.1770 us | 3.3389 us | 41.977 us |  5.27 |    0.65 |    3 | 3.3569 |     - |     - |  10.38 KB |
| DataContractResolverIgnoreAncestors | 30.842 us | 0.7237 us | 2.0531 us | 30.738 us |  3.79 |    0.39 |    2 | 1.7700 |     - |     - |   5.57 KB |
|                     JsonNetResolver | 56.316 us | 1.5730 us | 4.5386 us | 55.187 us |  6.93 |    0.79 |    4 | 5.3711 |     - |     - |  16.58 KB |

### Convert a dictionary to an instance (v1.1.0)

.. 

### Convert a dictionary to an instance (v2.0.0)

|                              Method |      Mean |    Error |    StdDev | Ratio | RatioSD | Rank |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------:|---------:|----------:|------:|--------:|-----:|--------:|------:|------:|----------:|
|                     DefaultResolver |  31.00 us | 0.617 us |  0.660 us |  1.00 |    0.00 |    1 |  2.0752 |     - |     - |   6.54 KB |
|                DataContractResolver | 358.70 us | 7.142 us | 11.120 us | 11.62 |    0.41 |    3 | 28.8086 |     - |     - |  88.28 KB |
| DataContractResolverIgnoreAncestors | 261.06 us | 5.195 us |  9.628 us |  8.42 |    0.35 |    2 | 11.2305 |     - |     - |  35.48 KB |
|                     JsonNetResolver | 424.18 us | 9.218 us | 26.001 us | 13.95 |    1.08 |    4 | 37.1094 |     - |     - | 116.48 KB |

## Open Source License Acknowledgements and Third-Party Copyrights

- Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
