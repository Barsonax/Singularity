# Singularity
[![Discord](https://img.shields.io/discord/569232642105540608.svg)](https://discord.gg/cKFnjjk) [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) [![Build Status](https://dev.azure.com/Barsonax/Singularity/_apis/build/status/Singularity-CI?branchName=master)](https://dev.azure.com/Barsonax/Singularity/_build/latest?definitionId=7&branchName=master) ![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/Barsonax/Singularity/7/master.svg) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=security_rating)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) ![coverage](https://img.shields.io/azure-devops/coverage/Barsonax/Singularity/7/master.svg) [![Beerpay](https://img.shields.io/beerpay/Barsonax/Singularity.svg)](https://beerpay.io/Barsonax/Singularity)

## Features
- Extreme performance, Singularity is one of the fastest if not the fastest dependency injection container out there. Don't believe me? Check out my [benchmarks](#Benchmarks) or if you want a second opinion check out the benchmarks that Daniel Palme made [here](https://github.com/danielpalme/IocPerformance).
- Clean fluent API.
- Generic wrappers:
  1. `Func<T>`
  1. `Lazy<T>`
  1. `Expression<Func<T>>`
- Collection support:
  1. `IEnumerable<T>`
  1. `IReadOnlyCollection<T>`
  1. `IReadOnlyList`
- Supports open generics.
- Supports resolving unregistered concrete types.
- Supports decorators.
- Supports method and property injection without forcing you to litter attributes all over your code base. All configuration is kept inside the container.
- Auto dispose, this is off by default but can be turned on with `With(DisposeBehavior)`or by setting `SingularitySettings.AutoDispose` to true.
- Custom finalizers with the `WithFinalizer(Action<TInstance>)` method.
- Supports Transient, Singleton and Scope lifetimes.
- Supports child containers.
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
BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.437 (1809/October2018Update/Redstone5)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview3-010431
  [Host]       : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
  LegacyJitX64 : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0
  RyuJitX64    : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT

Platform=X64  IterationTime=500.0000 ms

|    Method |          Job |       Jit | Runtime |      Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------- |------------- |---------- |-------- |----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
| Singleton | LegacyJitX64 | LegacyJit |     Clr |  7.700 ns | 0.0722 ns | 0.0675 ns |           - |           - |           - |                   - |
| Transient | LegacyJitX64 | LegacyJit |     Clr | 10.086 ns | 0.0220 ns | 0.0195 ns |      0.0057 |           - |           - |                24 B |
|  Combined | LegacyJitX64 | LegacyJit |     Clr | 15.067 ns | 0.0568 ns | 0.0532 ns |      0.0133 |           - |           - |                56 B |
|   Complex | LegacyJitX64 | LegacyJit |     Clr | 26.822 ns | 0.1198 ns | 0.1121 ns |      0.0229 |           - |           - |                96 B |
| Singleton |    RyuJitX64 |    RyuJit |    Core |  7.744 ns | 0.0743 ns | 0.0695 ns |           - |           - |           - |                   - |
| Transient |    RyuJitX64 |    RyuJit |    Core | 10.031 ns | 0.0172 ns | 0.0161 ns |      0.0057 |           - |           - |                24 B |
|  Combined |    RyuJitX64 |    RyuJit |    Core | 16.454 ns | 0.0564 ns | 0.0500 ns |      0.0133 |           - |           - |                56 B |
|   Complex |    RyuJitX64 |    RyuJit |    Core | 23.667 ns | 0.0410 ns | 0.0342 ns |      0.0229 |           - |           - |                96 B |
```

### Nuget

| Library | Version |
|-------------|--------|
| Singularity      | [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) |
| Singularity.Duality.core      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.Duality.core)](https://www.nuget.org/packages/Singularity.Duality.core/)|
| Singularity.Microsoft.DependencyInjection      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.Microsoft.DependencyInjection)](https://www.nuget.org/packages/Singularity.Microsoft.DependencyInjection/)|

### Random info
![GitHub repo size](https://img.shields.io/github/repo-size/Barsonax/Singularity.svg) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/barsonax/singularity.svg) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=ncloc)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity) [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Barsonax_Singularity&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=Barsonax_Singularity)


[![Build History](https://buildstats.info/azurepipelines/chart/Barsonax/Singularity/7?branch=master)](https://dev.azure.com/Barsonax/Singularity/_build?definitionId=7)

### Donations
Support me by buying a beer [![Beerpay](https://img.shields.io/beerpay/Barsonax/Singularity.svg)](https://beerpay.io/Barsonax/Singularity)
 test5