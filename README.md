<img width="64px" src="https://github.com/Barsonax/Singularity/blob/master/src/Icon.png" />

# Singularity
[![Discord](https://img.shields.io/discord/569232642105540608.svg)](https://discord.gg/cKFnjjk) [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) [![Build Status](https://dev.azure.com/Barsonax/Singularity/_apis/build/status/Singularity-CI?branchName=master&stageName=Build)](https://dev.azure.com/Barsonax/Singularity/_build/latest?definitionId=7&branchName=master) ![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/Barsonax/Singularity/7/master.svg) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=security_rating)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) ![coverage](https://img.shields.io/azure-devops/coverage/Barsonax/Singularity/7/master.svg) [![Beerpay](https://img.shields.io/beerpay/Barsonax/Singularity.svg)](https://beerpay.io/Barsonax/Singularity)

## Features
- Extreme performance, Singularity is one of the fastest if not the fastest dependency injection container out there. Don't believe me? Check out my [benchmarks](#Benchmarks) or if you want a second opinion check out the benchmarks that Daniel Palme made [here](https://github.com/danielpalme/IocPerformance).
- Clean fluent API.
- [Source Link](https://github.com/dotnet/sourcelink) enabled
- Generic wrappers:
  1. `Func<T>`
  1. `Lazy<T>`
  1. `Expression<Func<T>>`
  1. And any other generic wrapper you may have defined yourself.
- Collection support:
  1. `IEnumerable<T>`
  1. `IReadOnlyCollection<T>`
  1. `IReadOnlyList<T>`
  1. `T[]`
  1. `List<T>`
  1. `ICollection<T>`
  1. `IList<T>`
  1. `HashSet<T>`
  1. `ISet<T>`
- Supports open generics.
- Supports resolving unregistered concrete types.
- Supports decorators.
- Supports method and property injection without forcing you to litter attributes all over your code base. All configuration is kept inside the container.
- Supports dynamically picking the most suitable constructor based on the available types that can be resolved.
- Auto dispose, this is off by default but can be turned on with `With(DisposeBehavior)`or adding the lifetimes you want to auto dispose to `SingularitySettings.AutoDisposeLifetimes`.
- Custom finalizers with the `WithFinalizer(Action<TInstance>)` method.
- Supports Transient, Singleton and Scope lifetimes.
- Supports child containers.
- Supports best fit constructor selection
- Clear error messages and fail fast to point you in the right direction as fast as possible.

## Getting started
### Installation
`Singularity` can be installed through nuget. The packages that are available can be found in the [nuget](#nuget) section 

### A simple example
Its easy to setup a container and request a instance:
```cs
var container = new Container(builder =>
{
    builder.Register<ITestService10, TestService10>();
});

var instance = container.GetInstance<ITestService10>();
```
However `Singularity` can do much more than this simple example. You can request the instance with different wrapper types such as `Lazy<T>`:
```cs
var lazyInstance = container.GetInstance<Lazy<ITestService10>>();
```
Or you can request the factory to create the instance:
```cs
var factory = container.GetInstance<Func<ITestService10>>();
```
You can even request the expression itself:
```cs
var instanceExpression = container.GetInstance<Expression<Func<ITestService10>>>();
```
Ofcourse its possible to combine these with for instance a collection type such as IEnumerable<T> or IReadOnlyList<T>:
```cs
var instanceExpressions = container.GetInstance<IReadOnlyList<Expression<Func<IPlugin>>>>(); //Returns all expressions for IPlugin registrations
```

Advanced scenarios such as open generics are also supported. 

## Documentation
More info about `Singularity` can be found on the documentation website which can be found [here](https://barsonax.github.io/Singularity.Docs/).

## Other
### Benchmarks
The code used in the benchmark can be found [here](https://github.com/Barsonax/Singularity/blob/master/Singularity.TestClasses/Benchmark/SimpleSingularityContainerBenchmark.cs)
```
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=3.1.200
  [Host]       : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT
  LegacyJitX64 : .NET Framework 4.8 (4.8.4150.0), X64 RyuJIT
  RyuJitX64    : .NET Core 2.1.16 (CoreCLR 4.6.28516.03, CoreFX 4.6.28516.10), X64 RyuJIT

Platform=X64  IterationTime=1.0000 s

|    Method |          Job |       Jit |       Runtime |          Mean |       Error |      StdDev |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|---------- |------------- |---------- |-------------- |--------------:|------------:|------------:|-------:|-------:|------:|----------:|
|  Register | LegacyJitX64 | LegacyJit |    .NET 4.7.2 | 17,918.890 ns | 101.5440 ns |  94.9844 ns | 9.3947 |      - |     - |   15600 B |
| Singleton | LegacyJitX64 | LegacyJit |    .NET 4.7.2 |     10.539 ns |   0.1156 ns |   0.1082 ns |      - |      - |     - |         - |
| Transient | LegacyJitX64 | LegacyJit |    .NET 4.7.2 |     15.095 ns |   0.0562 ns |   0.0525 ns | 0.0145 |      - |     - |      24 B |
|  Combined | LegacyJitX64 | LegacyJit |    .NET 4.7.2 |     20.722 ns |   0.0707 ns |   0.0552 ns | 0.0338 |      - |     - |      56 B |
|   Complex | LegacyJitX64 | LegacyJit |    .NET 4.7.2 |     27.218 ns |   0.2894 ns |   0.2566 ns | 0.0580 |      - |     - |      96 B |
|  Register |    RyuJitX64 |    RyuJit | .NET Core 2.1 | 22,300.706 ns | 109.0636 ns | 102.0181 ns | 8.6912 |      - |     - |   14464 B |
| Singleton |    RyuJitX64 |    RyuJit | .NET Core 2.1 |      9.569 ns |   0.0634 ns |   0.0593 ns |      - |      - |     - |         - |
| Transient |    RyuJitX64 |    RyuJit | .NET Core 2.1 |     14.755 ns |   0.1025 ns |   0.0959 ns | 0.0145 |      - |     - |      24 B |
|  Combined |    RyuJitX64 |    RyuJit | .NET Core 2.1 |     22.506 ns |   0.3656 ns |   0.3420 ns | 0.0337 |      - |     - |      56 B |
|   Complex |    RyuJitX64 |    RyuJit | .NET Core 2.1 |     26.756 ns |   0.3521 ns |   0.3294 ns | 0.0578 |      - |     - |      96 B |
|  Register |    RyuJitX64 |    RyuJit | .NET Core 3.1 | 15,413.451 ns |  67.0614 ns |  62.7292 ns | 1.7130 | 0.0309 |     - |   14347 B |
| Singleton |    RyuJitX64 |    RyuJit | .NET Core 3.1 |     12.167 ns |   0.1213 ns |   0.1135 ns |      - |      - |     - |         - |
| Transient |    RyuJitX64 |    RyuJit | .NET Core 3.1 |     14.461 ns |   0.0698 ns |   0.0653 ns | 0.0029 |      - |     - |      24 B |
|  Combined |    RyuJitX64 |    RyuJit | .NET Core 3.1 |     20.933 ns |   0.2406 ns |   0.2251 ns | 0.0067 |      - |     - |      56 B |
|   Complex |    RyuJitX64 |    RyuJit | .NET Core 3.1 |     24.196 ns |   0.1059 ns |   0.0991 ns | 0.0115 |      - |     - |      96 B |
```

### Nuget

| Library | Version |
|-------------|--------|
| Singularity      | [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) |
| Singularity.Duality.core      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.Duality.core)](https://www.nuget.org/packages/Singularity.Duality.core/)|
| Singularity.Microsoft.DependencyInjection      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.Microsoft.DependencyInjection)](https://www.nuget.org/packages/Singularity.Microsoft.DependencyInjection/)|
| Singularity.AspNetCore.Hosting      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.AspNetCore.Hosting)](https://www.nuget.org/packages/Singularity.AspNetCore.Hosting/)|
| Singularity.AspNetCore.MVC      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.AspNetCore.MVC)](https://www.nuget.org/packages/Singularity.AspNetCore.MVC/)|

### Random info
![GitHub repo size](https://img.shields.io/github/repo-size/Barsonax/Singularity.svg) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/barsonax/singularity.svg) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=ncloc)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity)


[![Build History](https://buildstats.info/azurepipelines/chart/Barsonax/Singularity/7?branch=master)](https://dev.azure.com/Barsonax/Singularity/_build?definitionId=7)

### Donations
| Paypal | Beerpay |
|-------------|--------|
|[![paypal](https://www.paypalobjects.com/en_US/NL/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=VTXT9EBQ3CF5E)|[![Beerpay](https://img.shields.io/beerpay/Barsonax/Singularity.svg)](https://beerpay.io/Barsonax/Singularity)|

### Licensing
Licensed under LGPL.
