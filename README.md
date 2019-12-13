# Extensions.Dictionary

[![NuGet](https://img.shields.io/nuget/v/Extensions.Dictionary.svg)](https://www.nuget.org/packages/Extensions.Dictionary)
[![Azure DevOps](https://dev.azure.com/SiberaIndustries/Extensions.Dictionary/_apis/build/status/SiberaIndustries.Extensions.Dictionary)](https://dev.azure.com/SiberaIndustries/Extensions.Dictionary)
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
var settings = new ConverterSettings { Resolver = new DataContractResolver() };
var dictionary2 = person.ToDictionary(settings;

// dictionary1: <Firstname = foo; Lastname = bar>
// dictionary2: <Name1 = foo>
```

Convert a dictionary back to it's typed instance:

```cs
// Option 1
var person1 = dictionary.ToInstance<Person>();

// Option 2
var settings = new ConverterSettings { Resolver = new DataContractResolver() };
var person2 = dictionary.ToInstance<Person>(settings);
```

## Extensibility

### Custom MemberConverter

This sample creates a custom converter from `MemberConverter<T>` that overrides conversion for the `System.Numerics.Vector3` class.

```cs
public class Vector3Converter : MemberConverter<Vector3>
{
    public override IDictionary<string, object> ToDictionary(Vector3 value, ConverterSettings settings)
    {
        return new Dictionary<string, object>(3)
        {
            { nameof(Vector3.X), value.X },
            { nameof(Vector3.Y), value.Y },
            { nameof(Vector3.Z), value.Z },
        };
    }

    public override Vector3 ToInstance(IDictionary<string, object?> value, ConverterSettings settings)
    {
        return new Vector3
        {
            X = float.Parse(value[nameof(Vector3.X)]?.ToString(), settings.Culture),
            Y = float.Parse(value[nameof(Vector3.Y)]?.ToString(), settings.Culture),
            Z = float.Parse(value[nameof(Vector3.Z)]?.ToString(), settings.Culture),
        };
    }
}
```

```cs
// Usage
var plane = new Plane(1f, 1f, 1f, .1f);

var settings = new ConverterSettings();
settings.Converters.Add(new Vector3Converter());

var dict = plane.ToDictionary(settings);
```

### Custom Resolver

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

|                              Method |   N |      Mean |     Error |    StdDev | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |---- |----------:|----------:|----------:|------:|--------:|-----:|-------:|------:|------:|----------:|
|                     DefaultResolver |   1 |  6.277 us | 12.242 us | 0.6710 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver |   1 | 16.106 us | 18.827 us | 1.0320 us |  2.59 |    0.35 |    3 | 0.9918 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors |   1 | 12.773 us |  8.038 us | 0.4406 us |  2.05 |    0.15 |    2 | 0.9766 |     - |     - |   3.07 KB |
|                     JsonNetResolver |   1 | 50.861 us | 56.199 us | 3.0805 us |  8.14 |    0.56 |    4 | 5.8594 |     - |     - |     18 KB |
|                                     |     |           |           |           |       |         |      |        |       |       |           |
|                     DefaultResolver |  10 |  5.572 us |  1.225 us | 0.0672 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver |  10 | 14.726 us | 33.944 us | 1.8606 us |  2.64 |    0.32 |    3 | 0.9918 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors |  10 | 12.287 us |  1.493 us | 0.0818 us |  2.21 |    0.03 |    2 | 0.9918 |     - |     - |   3.07 KB |
|                     JsonNetResolver |  10 | 52.918 us | 89.842 us | 4.9246 us |  9.50 |    1.00 |    4 | 5.8594 |     - |     - |     18 KB |
|                                     |     |           |           |           |       |         |      |        |       |       |           |
|                     DefaultResolver | 100 |  6.063 us |  4.733 us | 0.2595 us |  1.00 |    0.00 |    1 | 0.5875 |     - |     - |   1.81 KB |
|                DataContractResolver | 100 | 12.419 us |  4.004 us | 0.2195 us |  2.05 |    0.06 |    2 | 0.9918 |     - |     - |   3.07 KB |
| DataContractResolver-IgnoreAncestors | 100 | 12.413 us | 13.096 us | 0.7179 us |  2.05 |    0.20 |    2 | 0.9918 |     - |     - |   3.07 KB |
|                     JsonNetResolver | 100 | 54.870 us | 36.022 us | 1.9745 us |  9.06 |    0.50 |    3 | 5.8594 |     - |     - |     18 KB |

### Convert to instance

|                              Method |   N |        Mean |        Error |     StdDev |  Ratio | RatioSD | Rank |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |---- |------------:|-------------:|-----------:|-------:|--------:|-----:|--------:|------:|------:|----------:|
|                     DefaultResolver |   1 |    10.76 us |     0.491 us |   0.027 us |   1.00 |    0.00 |    1 |  0.7019 |     - |     - |   2.18 KB |
|                DataContractResolver |   1 |    53.33 us |    61.631 us |   3.378 us |   4.96 |    0.32 |    3 |  3.7842 |     - |     - |  11.72 KB |
| DataContractResolver-IgnoreAncestors |   1 |    54.74 us |    99.851 us |   5.473 us |   5.09 |    0.51 |    4 |  3.7842 |     - |     - |  11.72 KB |
|                     JsonNetResolver |   1 |   161.86 us |    94.736 us |   5.193 us |  15.05 |    0.46 |    5 | 16.6016 |     - |     - |  51.52 KB |
|                         PureJsonNet |   1 |    18.55 us |    24.079 us |   1.320 us |   1.72 |    0.12 |    2 |  2.4414 |     - |     - |   7.49 KB |
|                                     |     |             |              |            |        |         |      |         |       |       |           |
|                     DefaultResolver |  10 |    11.64 us |    18.704 us |   1.025 us |   1.00 |    0.00 |    1 |  0.7019 |     - |     - |   2.18 KB |
|                DataContractResolver |  10 |    64.32 us |   118.677 us |   6.505 us |   5.53 |    0.42 |    3 |  3.7842 |     - |     - |  11.72 KB |
| DataContractResolver-IgnoreAncestors |  10 |    54.44 us |    22.665 us |   1.242 us |   4.70 |    0.40 |    2 |  3.7842 |     - |     - |  11.72 KB |
|                     JsonNetResolver |  10 |   169.81 us |   208.025 us |  11.403 us |  14.66 |    1.68 |    5 | 16.6016 |     - |     - |  51.52 KB |
|                         PureJsonNet |  10 |   163.40 us |   185.507 us |  10.168 us |  14.10 |    1.48 |    4 | 11.2305 |     - |     - |  35.09 KB |
|                                     |     |             |              |            |        |         |      |         |       |       |           |
|                     DefaultResolver | 100 |    13.51 us |    13.977 us |   0.766 us |   1.00 |    0.00 |    1 |  0.7019 |     - |     - |   2.18 KB |
|                DataContractResolver | 100 |    64.19 us |    64.332 us |   3.526 us |   4.76 |    0.36 |    3 |  3.7842 |     - |     - |  11.72 KB |
| DataContractResolver-IgnoreAncestors | 100 |    56.80 us |    92.891 us |   5.092 us |   4.20 |    0.28 |    2 |  3.7842 |     - |     - |  11.72 KB |
|                     JsonNetResolver | 100 |   138.36 us |    34.188 us |   1.874 us |  10.26 |    0.44 |    4 | 16.6016 |     - |     - |  51.52 KB |
|                         PureJsonNet | 100 | 1,858.79 us | 6,022.705 us | 330.125 us | 137.79 |   24.91 |    5 | 93.7500 |     - |     - | 292.02 KB |

## Open Source License Acknowledgements and Third-Party Copyrights

- Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
