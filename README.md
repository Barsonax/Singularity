# Singularity
Singularity is a ioc container that focuses on the following things
- Very high performance. Once the container is configured injecting dependencies is done by a dictionary lookup and simply invoking a delegate. This means that performance in Singularity is measured in nanoseconds as opposed to microseconds in other containers. This makes it feasible to use singularity in applications where performance matters such as games. 
- No magic. Singularity has been designed in such a way that it won't hide too much from you. For instance `Dispose` wont be automagically called but instead you can configure Singularity to do so through the `OnDeath` method. This way you can always find out who is calling your methods.
- Clear error messages and fail fast to point you in the right direction as fast as possible.

## Getting started
### Installation
`Singularity` can be installed through nuget. The packages that are available can be found in the nuget section 

### A simple example
Its easy to setup a container and request a instance:
```cs
var config = new BindingConfig();
config.For<ITestService10>().Inject<TestService10>();

var container = new Container(config);

var value = container.GetInstance<ITestService10>();
```
However `Singularity` can do much more than this simple example

# Documentation
More info about `Singularity` can be found on the wiki. I suggest you to start [here](https://github.com/Barsonax/Singularity/wiki/Configuring-Dependencies). 

For duality users there is also a guide on how to use `Singularity.Duality` in duality [here](https://github.com/Barsonax/Singularity/wiki/Using-Singularity-in-Duality).

## Other
### Build status
| Branch | Status |
|-------------|--------|
| master      | [![Build status](https://ci.appveyor.com/api/projects/status/7fp2lnmhmgld0l37/branch/master?svg=true)](https://ci.appveyor.com/project/Barsonax/singularity/branch/master) |
| develop      | [![Build status](https://ci.appveyor.com/api/projects/status/7fp2lnmhmgld0l37/branch/develop?svg=true)](https://ci.appveyor.com/project/Barsonax/singularity/branch/develop) |

### Nuget

| Library | Version |
|-------------|--------|
| Singularity      | [![NuGet Badge](https://buildstats.info/nuget/Singularity)](https://www.nuget.org/packages/Singularity/) |
| Singularity.Duality.core      | [![NuGet Badge](https://buildstats.info/nuget/Singularity.Duality.core)](https://www.nuget.org/packages/Singularity.Duality.core/)|
