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

// Option 2: Same as option 1 + respect DataContract attributes (DataMember / IgnoreDataMember)
var dictionary2 = person.ToDictionary(new DataContractResolver());

// dictionary1: <Firstname = foo; Lastname = bar>
// dictionary2: <Name1 = foo>
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

|                              Method |   N |      Mean |       Error |    StdDev | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |---- |----------:|------------:|----------:|------:|--------:|-----:|-------:|------:|------:|----------:|
|                     DefaultResolver |   1 |  7.453 us |   9.8603 us | 0.5405 us |  1.00 |    0.00 |    1 | 0.5798 |     - |     - |   1.81 KB |
|                DataContractResolver |   1 | 14.038 us |  16.3036 us | 0.8937 us |  1.89 |    0.20 |    3 | 0.9918 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors |   1 | 13.784 us |   0.4209 us | 0.0231 us |  1.86 |    0.13 |    2 | 0.9918 |     - |     - |   3.07 KB |
|                     JsonNetResolver |   1 | 59.822 us |  48.8324 us | 2.6767 us |  8.05 |    0.64 |    4 | 5.8594 |     - |     - |     18 KB |
|                                     |     |           |             |           |       |         |      |        |       |       |           |
|                     DefaultResolver |  10 |  6.866 us |  10.7596 us | 0.5898 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver |  10 | 17.311 us |  31.3831 us | 1.7202 us |  2.53 |    0.28 |    3 | 0.9918 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors |  10 | 15.352 us |  10.4117 us | 0.5707 us |  2.24 |    0.12 |    2 | 0.9918 |     - |     - |   3.07 KB |
|                     JsonNetResolver |  10 | 61.376 us | 153.7049 us | 8.4251 us |  9.02 |    1.80 |    4 | 5.8594 |     - |     - |     18 KB |
|                                     |     |           |             |           |       |         |      |        |       |       |           |
|                     DefaultResolver | 100 |  6.744 us |   6.8390 us | 0.3749 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver | 100 | 16.925 us |   2.2942 us | 0.1258 us |  2.52 |    0.15 |    3 | 0.9766 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors | 100 | 16.271 us |  22.4270 us | 1.2293 us |  2.41 |    0.12 |    2 | 0.9918 |     - |     - |   3.07 KB |
|                     JsonNetResolver | 100 | 56.603 us |  17.5337 us | 0.9611 us |  8.41 |    0.36 |    4 | 5.8594 |     - |     - |     18 KB |

### Convert to instance

|                              Method |   N |         Mean |       Error |     StdDev |  Ratio | RatioSD | Rank |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |---- |-------------:|------------:|-----------:|-------:|--------:|-----:|--------:|------:|------:|----------:|
|                     DefaultResolver |   1 |     8.686 us |   4.8490 us |  0.2658 us |   1.00 |    0.00 |    1 |  0.6104 |     - |     - |   1.88 KB |
|                DataContractResolver |   1 |    51.797 us |  46.7753 us |  2.5639 us |   5.97 |    0.41 |    4 |  3.2349 |     - |     - |   9.98 KB |
| DataContractResolver-IgnoreAncestors |   1 |    42.446 us |  45.2487 us |  2.4802 us |   4.89 |    0.21 |    3 |  3.2349 |     - |     - |   9.98 KB |
|                     JsonNetResolver |   1 |   121.708 us | 158.1717 us |  8.6699 us |  14.00 |    0.61 |    5 | 14.1602 |     - |     - |   43.7 KB |
|                         PureJsonNet |   1 |    14.295 us |   1.9047 us |  0.1044 us |   1.65 |    0.04 |    2 |  2.3041 |     - |     - |    7.1 KB |
|                                     |     |              |             |            |        |         |      |         |       |       |           |
|                     DefaultResolver |  10 |     8.216 us |   0.8129 us |  0.0446 us |   1.00 |    0.00 |    1 |  0.6104 |     - |     - |   1.88 KB |
|                DataContractResolver |  10 |    40.075 us |  28.5257 us |  1.5636 us |   4.88 |    0.17 |    2 |  3.2349 |     - |     - |   9.98 KB |
| DataContractResolver-IgnoreAncestors |  10 |    45.841 us |  19.7826 us |  1.0844 us |   5.58 |    0.16 |    3 |  3.2349 |     - |     - |   9.98 KB |
|                     JsonNetResolver |  10 |   108.532 us |  25.2108 us |  1.3819 us |  13.21 |    0.24 |    4 | 14.1602 |     - |     - |   43.7 KB |
|                         PureJsonNet |  10 |   122.506 us |  45.3597 us |  2.4863 us |  14.91 |    0.24 |    5 | 10.4980 |     - |     - |  32.43 KB |
|                                     |     |              |             |            |        |         |      |         |       |       |           |
|                     DefaultResolver | 100 |     8.072 us |   9.1833 us |  0.5034 us |   1.00 |    0.00 |    1 |  0.6104 |     - |     - |   1.88 KB |
|                DataContractResolver | 100 |    43.830 us |  92.1332 us |  5.0501 us |   5.47 |    0.94 |    3 |  3.2349 |     - |     - |   9.98 KB |
| DataContractResolver-IgnoreAncestors | 100 |    39.588 us |   4.9575 us |  0.2717 us |   4.92 |    0.31 |    2 |  3.2349 |     - |     - |   9.98 KB |
|                     JsonNetResolver | 100 |   115.840 us | 181.2204 us |  9.9333 us |  14.44 |    2.20 |    4 | 14.1602 |     - |     - |   43.7 KB |
|                         PureJsonNet | 100 | 1,305.145 us | 808.9375 us | 44.3406 us | 162.27 |   14.66 |    5 | 85.9375 |     - |     - | 266.69 KB |

## Open Source License Acknowledgements and Third-Party Copyrights

- Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
