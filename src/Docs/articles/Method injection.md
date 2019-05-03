## Why
While not recommended sometimes you cannot get away with using just constructor injection and you have to use a different way to inject your dependencies. 
For instance in duality the component instances are not managed by the container but by the game engine. This makes it impossible to use constructor injection as the container is not the one that is creating the component instances.


## How
Singularity supports method and property injection through the `LateInject` method:
```cs
var container = new Container(builder =>
{
    builder.Register<ITestService10, TestService10>();
    builder.LateInject<LateInjectionClass>(c => c
		.UseMethod(nameof(LateInjectionClass.Inject)));
        .UseProperty(nameof(LateInjectionClass.TestService10Settable)));
});

var instance = new LateInjectionClass();
container.LateInject(instance);
```

When `LateInject` is called the container will inject the required instances in the registered methods and properties. 
As you can see Singularity does not require you to take a dependency on the container like some other containers.
You can see this in the definition of the LateInjectionClass which just a simple class:
```cs
public class LateInjectionClass
{
    public ITestService10 TestService10 { get; private set; }

	public ITestService10 TestService10Settable { get; set; }

    public void Inject(ITestService10 testService10)
    {
        TestService10 = testService10;
    }
}
```