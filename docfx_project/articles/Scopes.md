## Scopes
Contrary to some other ioc containers Singularity will not cleanup resources automagically for you. This makes it more clear when resources are cleaned up and also saves some performance when its not needed.
You can use the `OnDeath` method to execute code such as calling `Dispose` when the scope ends:
```cs
config.Register<ITestService10, TestService10>().OnDeath(instance => instance.Dispose());
```

A scope ends when the container is disposed. This means it will execute the OnDeath actions on all created instances. You can create a new scope by creating a nested container and passing in a new scope:
You can create a new scope this way:
```cs
var config = new BindingConfig();
config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());

var container = new Container(config);

Scoped scope = container.BeginScope();
var disposable = scope.GetInstance<IDisposable>();
scope.Dispose(); //disposable will now get disposed
```

Creating a container will also create a implicit scope:
```cs
var config = new BindingConfig();
var container = new Container(config);

var nestedConfig = new BindingConfig();
Container nestedContainer = container.GetNestedContainer(nestedConfig);

var disposable = container.GetInstance<IDisposable>();
var disposableNested = nestedContainer.GetInstance<IDisposable>();
nestedContainer.Dispose(); //disposableNested will now get disposed
container.Dispose(); //disposable will now get disposed
```