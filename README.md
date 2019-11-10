# Extensions.Dictionary

[![NuGet](https://img.shields.io/nuget/v/Extensions.Dictionary.svg)](https://www.nuget.org/packages/Extensions.Dictionary)
[![Build status](https://ci.appveyor.com/api/projects/status/o8eyfg4065t5qii9/branch/master?svg=true)](https://ci.appveyor.com/project/SiberaIndustries/extensions-dictionary/branch/master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_Extensions.Dictionary&metric=alert_status)](https://sonarcloud.io/dashboard?id=SiberaIndustries_Extensions.Dictionary)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_Extensions.Dictionary&metric=coverage)](https://sonarcloud.io/dashboard?id=SiberaIndustries_Extensions.Dictionary)

## Introduction

The easiest way to start using `Extensions.Dictionary` is to install it’s Nuget package. In Visual Studio open Package Manager console and type the following:

```cs
Install-Package Extensions.Dictionary
```

In the source file where you will be using Dictionary Extensions import the namespace:

```cs
using Extensions.Dictionary;
```

Sample class:

```cs
[DataContract]
public class Person
{
    [DataMember(Name = "Name1")]
    public string Firstname { get; set; }

    [IgnoreDataMember]
    public string Lastname { get; set; }
}
```

Convert an instance to a dictionary:

```cs
var person = new Person { Firstname = "foo", Lastname = "bar" };

// Option 1: Convert all public properties and instance fields
var dictionary1 = person.ToDictionary();

// result:
// { 
//     { "Firstname" = "foo" },
//     { "Lastname" = "bar" }
// }

// Option 2: Same as option 1 + respect DataContract attributes (DataMember / IgnoreDataMember)
var dictionary2 = person.ToDictionary(new DataContractResolver());

// result:
// { 
//     { "Name1" = "foo" }
// }
```

Convert a dictionary back to it's typed instance:

```cs
// Option 1
var person1 = dictionary.ToInstance<Person>();

// Option 2
var person2 = dictionary.ToInstance<Person>(new DataContractResolver());
```

## Extensibility

It is possible to customize the member name / value extraction. Just implement the provided `ISerializerResolver` interface. The following code snipped shows an example of a [Json.NET](https://www.newtonsoft.com/json) resolver:

```cs
public class JsonNetResolver : ISerializerResolver
{
    public string GetMemberName(MemberInfo memberInfo)
    {
        if (memberInfo == null)
        {
            throw new ArgumentNullException(nameof(memberInfo));
        }

        var attribute = memberInfo.GetCustomAttribute<JsonPropertyAttribute>();
        return attribute?.PropertyName ?? memberInfo.Name;
    }

    public object GetMemberValue(MemberInfo memberInfo, object instance)
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

    public MemberInfo[] GetMemberInfos(Type type)
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

The following benchmarks are showing the differences between converting from dictionary to instance and from instance to dictionary. SPOILER: You get the best efficiency with the `DefaultResolver`.

**Test environment**

``` ini
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 8 physical cores
.NET Core SDK=3.0.100
  [Host]   : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  ShortRun : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT 
```

### Convert to dictionary

|                              Method |   N |         Mean |        Error |      StdDev | Ratio | RatioSD | Rank |    Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|------------------------------------ |---- |-------------:|-------------:|------------:|------:|--------:|-----:|---------:|--------:|------:|-----------:|
|                     DefaultResolver |   1 |     8.476 us |     4.071 us |   0.2231 us |  1.00 |    0.00 |    1 |   0.7629 |       - |     - |    2.35 KB |
|                DataContractResolver |   1 |    14.066 us |     7.840 us |   0.4298 us |  1.66 |    0.09 |    2 |   1.1902 |       - |     - |    3.66 KB |
| DataContractResolverIgnoreAncestors |   1 |    15.612 us |    27.879 us |   1.5281 us |  1.84 |    0.16 |    3 |   1.1902 |       - |     - |    3.66 KB |
|                     JsonNetResolver |   1 |    61.831 us |    59.470 us |   3.2597 us |  7.29 |    0.29 |    4 |   5.6152 |       - |     - |   17.21 KB |
|                                     |     |              |              |             |       |         |      |          |         |       |            |
|                     DefaultResolver |  10 |    75.555 us |     9.707 us |   0.5321 us |  1.00 |    0.00 |    1 |   7.6904 |       - |     - |   23.82 KB |
|                DataContractResolver |  10 |   134.207 us |    33.168 us |   1.8180 us |  1.78 |    0.03 |    2 |  11.9629 |       - |     - |   36.88 KB |
| DataContractResolverIgnoreAncestors |  10 |   142.756 us |    31.539 us |   1.7288 us |  1.89 |    0.03 |    3 |  11.9629 |       - |     - |   36.88 KB |
|                     JsonNetResolver |  10 |   474.820 us |   173.911 us |   9.5326 us |  6.28 |    0.11 |    4 |  55.6641 |       - |     - |  172.55 KB |
|                                     |     |              |              |             |       |         |      |          |         |       |            |
|                     DefaultResolver | 100 |   818.978 us |   813.624 us |  44.5975 us |  1.00 |    0.00 |    1 |  69.3359 | 16.6016 |     - |  238.46 KB |
|                DataContractResolver | 100 | 1,565.105 us | 1,005.751 us |  55.1286 us |  1.91 |    0.08 |    3 | 105.4688 | 29.2969 |     - |  369.04 KB |
| DataContractResolverIgnoreAncestors | 100 | 1,491.541 us |   568.329 us |  31.1520 us |  1.83 |    0.13 |    2 | 105.4688 | 23.4375 |     - |  369.05 KB |
|                     JsonNetResolver | 100 | 5,538.160 us | 5,166.537 us | 283.1953 us |  6.76 |    0.06 |    4 | 492.1875 | 78.1250 |     - | 1725.11 KB |

### To instance (v2.0.0)

|                              Method |   N |         Mean |       Error |     StdDev |  Ratio | RatioSD | Rank |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |---- |-------------:|------------:|-----------:|-------:|--------:|-----:|--------:|------:|------:|----------:|
|                     DefaultResolver |   1 |     8.686 us |   4.8490 us |  0.2658 us |   1.00 |    0.00 |    1 |  0.6104 |     - |     - |   1.88 KB |
|                DataContractResolver |   1 |    51.797 us |  46.7753 us |  2.5639 us |   5.97 |    0.41 |    4 |  3.2349 |     - |     - |   9.98 KB |
| DataContractResolverIgnoreAncestors |   1 |    42.446 us |  45.2487 us |  2.4802 us |   4.89 |    0.21 |    3 |  3.2349 |     - |     - |   9.98 KB |
|                     JsonNetResolver |   1 |   121.708 us | 158.1717 us |  8.6699 us |  14.00 |    0.61 |    5 | 14.1602 |     - |     - |   43.7 KB |
|                         PureJsonNet |   1 |    14.295 us |   1.9047 us |  0.1044 us |   1.65 |    0.04 |    2 |  2.3041 |     - |     - |    7.1 KB |
|                                     |     |              |             |            |        |         |      |         |       |       |           |
|                     DefaultResolver |  10 |     8.216 us |   0.8129 us |  0.0446 us |   1.00 |    0.00 |    1 |  0.6104 |     - |     - |   1.88 KB |
|                DataContractResolver |  10 |    40.075 us |  28.5257 us |  1.5636 us |   4.88 |    0.17 |    2 |  3.2349 |     - |     - |   9.98 KB |
| DataContractResolverIgnoreAncestors |  10 |    45.841 us |  19.7826 us |  1.0844 us |   5.58 |    0.16 |    3 |  3.2349 |     - |     - |   9.98 KB |
|                     JsonNetResolver |  10 |   108.532 us |  25.2108 us |  1.3819 us |  13.21 |    0.24 |    4 | 14.1602 |     - |     - |   43.7 KB |
|                         PureJsonNet |  10 |   122.506 us |  45.3597 us |  2.4863 us |  14.91 |    0.24 |    5 | 10.4980 |     - |     - |  32.43 KB |
|                                     |     |              |             |            |        |         |      |         |       |       |           |
|                     DefaultResolver | 100 |     8.072 us |   9.1833 us |  0.5034 us |   1.00 |    0.00 |    1 |  0.6104 |     - |     - |   1.88 KB |
|                DataContractResolver | 100 |    43.830 us |  92.1332 us |  5.0501 us |   5.47 |    0.94 |    3 |  3.2349 |     - |     - |   9.98 KB |
| DataContractResolverIgnoreAncestors | 100 |    39.588 us |   4.9575 us |  0.2717 us |   4.92 |    0.31 |    2 |  3.2349 |     - |     - |   9.98 KB |
|                     JsonNetResolver | 100 |   115.840 us | 181.2204 us |  9.9333 us |  14.44 |    2.20 |    4 | 14.1602 |     - |     - |   43.7 KB |
|                         PureJsonNet | 100 | 1,305.145 us | 808.9375 us | 44.3406 us | 162.27 |   14.66 |    5 | 85.9375 |     - |     - | 266.69 KB |

## Open Source License Acknowledgements and Third-Party Copyrights

- Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
