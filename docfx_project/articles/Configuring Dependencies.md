## Simple Dependencies
Setting up the container is straightforward and easy:
```cs
var container = new Container(builder =>
{
    builder.Register<ITestService10, TestService10>();
});
```

You can then request a instance by calling `GetInstance`:
```cs
var value = container.GetInstance<ITestService10>();
```
In this case a `TestService10` instance will be returned.

## Complex Dependencies
That wasnt that exciting right? All it did was calling the constructor to return a new instance. Lets say we have a slightly different config:
```cs
var container = new Container(builder =>
{
    builder.Register<ITestService10, TestService10>();
	builder.Register<ITestService11, TestService11>();
});
```
The `TestService11` class looks like this:
```cs
public class TestService11 : ITestService11
{
    public ITestService10 TestService10 { get; }
    public TestService11(ITestService10 testService10)
    {
        TestService10 = testService10;
    }
}
```

Now the container cannot simply call the constructor as the constructor needs another dependency. However its smart enough to understand that it has to use a new `TestService10` instance as parameter since we configured that it should inject such a instance whenever a `ITestService10` is needed. So the following will work:
```cs
var value = container.GetInstance<ITestService11>();
```

If there is only 1 constructor then Singularity will use that constructor's arguments types to determine what to inject. If there are more than 1 constructors then you need to use expressions to explicitly state what constructor you want:
```cs
var container = new Container(builder =>
{
    builder.Register<ITestService10, TestService10>();
	builder.Register<ITestService11>(c => c.Inject((ITestService10 testService10) => new TestService11(testService10)));
});
```
In this case the expression arguments will be used to figure out what the needed dependencies are.

## Where to go to from here?
I suggest you to read the article about lifetimes [here](https://github.com/Barsonax/Singularity/wiki/Lifetimes)