# Singularity
Singularity is a ioc container that focuses on the following things
- Very high performance. The first time you use a dependency it will be compiled to a delegate and cached in a dictionary. When that dependency is requested again all it then retrieves the delegate from the dictionary and invokes it. This means that performance in Singularity is measured in nanoseconds as opposed to microseconds like in other containers. This makes it feasible to use singularity in applications where performance matters such as games. 
- No magic. Singularity has been designed in such a way that it won't hide too much from you. For instance `Dispose` wont be automagically called but instead you can configure Singularity to do so through the `OnDeath` method. This way you can always find out who is calling your methods.
- Clear error messages and fail fast to point you in the right direction as fast as possible.

## Getting started
### Installation
`Singularity` can be installed through nuget. The packages that are available can be found in the [nuget](#nuget) section 

### A simple example
Its easy to setup a container and request a instance:
```cs
var config = new BindingConfig();
config.For<ITestService10>().Inject<TestService10>();

var container = new Container(config);

var value = container.GetInstance<ITestService10>();
```
However `Singularity` can do much more than this simple example

## Documentation
More info about `Singularity` can be found on the wiki. I suggest you to start [here](https://github.com/Barsonax/Singularity/wiki/Configuring-Dependencies). 

For duality users there is also a guide on how to use `Singularity.Duality` in duality [here](https://github.com/Barsonax/Singularity/wiki/Using-Singularity-in-Duality).

## Other


### Build status
| Branch | Status | Coverage |
|-------------|--------|-----|
| master      | [![Build status](https://ci.appveyor.com/api/projects/status/7fp2lnmhmgld0l37/branch/master?svg=true)](https://ci.appveyor.com/project/Barsonax/singularity/branch/master) | [![codecov](https://codecov.io/gh/Barsonax/Singularity/branch/master/graph/badge.svg)](https://codecov.io/gh/Barsonax/Singularity) |
| develop      | [![Build status](https://ci.appveyor.com/api/projects/status/7fp2lnmhmgld0l37/branch/develop?svg=true)](https://ci.appveyor.com/project/Barsonax/singularity/branch/develop) | [![codecov](https://codecov.io/gh/Barsonax/Singularity/branch/develop/graph/badge.svg)](https://codecov.io/gh/Barsonax/Singularity) |

### Nuget

| Library | Version |
|-------------|--------|
| Singularity      | [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) |
| Singularity.Duality.core      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.Duality.core)](https://www.nuget.org/packages/Singularity.Duality.core/)|
