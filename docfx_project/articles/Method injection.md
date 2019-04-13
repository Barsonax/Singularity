## Why
While not recommended sometimes you cannot get away with using just constructor injection and you have to use a different way to inject your dependencies. 
For instance in duality the component instances are not managed by the container but by the game engine. This makes it impossible to use constructor injection as the container is not the one that is creating the component instances.

## How
Singularity supports method injection through the `MethodInject` method:
```cs
var config = new BindingConfig();
config.For<ITestService10>().Inject<TestService10>();

var container = new Container(config);

var instance = new MethodInjectionClass();
container.MethodInject(instance);
```

The MethodInjectionClass is defined as follows:
```cs
public class MethodInjectionClass
{
    public ITestService10 TestService10 { get; private set; }

    [Inject]
    public void Inject(ITestService10 testService10)
    {
        TestService10 = testService10;
    }
}
```

When `MethodInject` is called the container will look for all methods that are marked with `[Inject]` and it will call those methods to provide the dependencies.