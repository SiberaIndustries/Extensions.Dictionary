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

|                              Method |   N |      Mean |      Error |    StdDev | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |---- |----------:|-----------:|----------:|------:|--------:|-----:|-------:|------:|------:|----------:|
|                     DefaultResolver |   1 |  6.903 us |   6.876 us | 0.3769 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver |   1 | 14.859 us |   3.213 us | 0.1761 us |  2.16 |    0.14 |    2 | 0.9918 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors |   1 | 15.035 us |  21.736 us | 1.1914 us |  2.18 |    0.20 |    3 | 0.9766 |     - |     - |   3.07 KB |
|                     JsonNetResolver |   1 | 64.778 us | 126.838 us | 6.9524 us |  9.37 |    0.70 |    4 | 5.8594 |     - |     - |     18 KB |
|                                     |     |           |            |           |       |         |      |        |       |       |           |
|                     DefaultResolver |  10 |  6.783 us |   7.680 us | 0.4210 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver |  10 | 15.318 us |   5.995 us | 0.3286 us |  2.27 |    0.19 |    3 | 0.9918 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors |  10 | 14.634 us |  15.415 us | 0.8449 us |  2.17 |    0.23 |    2 | 0.9918 |     - |     - |   3.07 KB |
|                     JsonNetResolver |  10 | 62.176 us |  70.114 us | 3.8432 us |  9.17 |    0.42 |    4 | 5.8594 |     - |     - |     18 KB |
|                                     |     |           |            |           |       |         |      |        |       |       |           |
|                     DefaultResolver | 100 |  6.943 us |   7.232 us | 0.3964 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver | 100 | 15.081 us |   5.606 us | 0.3073 us |  2.18 |    0.16 |    3 | 0.9766 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors | 100 | 14.863 us |   9.268 us | 0.5080 us |  2.15 |    0.19 |    2 | 0.9766 |     - |     - |   3.07 KB |
|                     JsonNetResolver | 100 | 74.002 us |  64.702 us | 3.5466 us | 10.70 |    1.11 |    4 | 5.8594 |     - |     - |     18 KB |

### Convert to instance

|                              Method |   N |        Mean |      Error |    StdDev |  Ratio | RatioSD | Rank |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |---- |------------:|-----------:|----------:|-------:|--------:|-----:|--------:|------:|------:|----------:|
|                     DefaultResolver |   1 |    13.91 us |   6.249 us |  0.343 us |   1.00 |    0.00 |    1 |  0.7629 |     - |     - |   2.38 KB |
|                DataContractResolver |   1 |    59.73 us |  22.023 us |  1.207 us |   4.30 |    0.19 |    3 |  3.8452 |     - |     - |  11.92 KB |
| DataContractResolver-IgnoreAncestors |   1 |    61.63 us |  69.315 us |  3.799 us |   4.43 |    0.29 |    4 |  3.8452 |     - |     - |  11.92 KB |
|                     JsonNetResolver |   1 |   187.46 us | 162.171 us |  8.889 us |  13.50 |    0.96 |    5 | 16.8457 |     - |     - |  51.71 KB |
|                         PureJsonNet |   1 |    21.30 us |  21.346 us |  1.170 us |   1.53 |    0.09 |    2 |  2.4414 |     - |     - |   7.49 KB |
|                                     |     |             |            |           |        |         |      |         |       |       |           |
|                     DefaultResolver |  10 |    15.52 us |  40.324 us |  2.210 us |   1.00 |    0.00 |    1 |  0.7629 |     - |     - |   2.38 KB |
|                DataContractResolver |  10 |    60.73 us | 138.117 us |  7.571 us |   3.96 |    0.62 |    3 |  3.8452 |     - |     - |  11.92 KB |
| DataContractResolver-IgnoreAncestors |  10 |    56.83 us |  27.725 us |  1.520 us |   3.73 |    0.67 |    2 |  3.7842 |     - |     - |  11.92 KB |
|                     JsonNetResolver |  10 |   174.38 us | 279.898 us | 15.342 us |  11.32 |    1.04 |    4 | 16.8457 |     - |     - |  51.71 KB |
|                         PureJsonNet |  10 |   176.40 us | 122.064 us |  6.691 us |  11.58 |    2.25 |    5 | 11.2305 |     - |     - |  35.09 KB |
|                                     |     |             |            |           |        |         |      |         |       |       |           |
|                     DefaultResolver | 100 |    13.22 us |  25.314 us |  1.388 us |   1.00 |    0.00 |    1 |  0.7629 |     - |     - |   2.38 KB |
|                DataContractResolver | 100 |    66.09 us | 108.747 us |  5.961 us |   5.00 |    0.20 |    3 |  3.8452 |     - |     - |  11.92 KB |
| DataContractResolver-IgnoreAncestors | 100 |    61.97 us |  78.508 us |  4.303 us |   4.72 |    0.55 |    2 |  3.8452 |     - |     - |  11.92 KB |
|                     JsonNetResolver | 100 |   189.03 us | 855.780 us | 46.908 us |  14.17 |    2.03 |    4 | 16.8457 |     - |     - |  51.71 KB |
|                         PureJsonNet | 100 | 1,707.06 us | 666.239 us | 36.519 us | 130.18 |   15.68 |    5 | 93.7500 |     - |     - | 292.02 KB |

## Open Source License Acknowledgements and Third-Party Copyrights

- Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
