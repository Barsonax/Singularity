# Home
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
config.Register<ITestService10>, TestService10>();

var container = new Container(config);

var value = container.GetInstance<ITestService10>();
```
However `Singularity` can do much more than this simple example. Advanced scenarios such as open generics are also supported.