# Home
[![Discord](https://img.shields.io/discord/569232642105540608.svg)](https://discord.gg/9x9J3y) [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) [![Build Status](https://dev.azure.com/Barsonax/Singularity/_apis/build/status/Singularity-CI?branchName=master)](https://dev.azure.com/Barsonax/Singularity/_build/latest?definitionId=7&branchName=master) ![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/Barsonax/Singularity/7/master.svg) ![coverage](https://img.shields.io/azure-devops/coverage/Barsonax/Singularity/7/master.svg) [![Beerpay](https://img.shields.io/beerpay/Barsonax/Singularity.svg)](https://beerpay.io/Barsonax/Singularity)

Singularity is a ioc container that focuses on the following things
- Performance, Singularity is one of the fastest if not the fastest dependency injection container out there. Don't believe me? Check out my [benchmarks](#Benchmarks) or if you want a second opinion check out the benchmarks that Daniel Palme made [here](https://github.com/danielpalme/IocPerformance).
- Not invasive. For instance `Dispose` wont be automatically called by default, instead you can configure Singularity to do so through the `With(DisposeBehavior)` method or tell singularity to automatically call `Dispose` by simply changing the settings. Is calling Dispose not enough and you want to invoke your own custom logic? The `WithFinalizer(Action<TInstance>)` method might be of help here.
- Clear error messages and fail fast to point you in the right direction as fast as possible.


## Getting started
### Installation
`Singularity` can be installed through nuget:
- [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) Singularity
- [![NuGet Badge](https://buildstats.info/nuget/Singularity.Duality.core)](https://www.nuget.org/packages/Singularity.Duality.core/) Singularity.Duality.core
- [![NuGet Badge](https://buildstats.info/nuget/Singularity.Microsoft.DependencyInjection)](https://www.nuget.org/packages/Singularity.Microsoft.DependencyInjection/) Singularity.Microsoft.DependencyInjection

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