## Lifetimes
Lifetimes allow you to control when new instances are created. You can set lifetimes with the `With` method:
```cs
var container = new Container(builder =>
{
    builder.Register<ITestService10, TestService10>(c => c.With(Lifetime.Transient));
});
```
And
```cs
var container = new Container(builder =>
{
    builder.Register<ITestService10, TestService10>(c => c.With(Lifetime.Singleton));
});
```

There are a few different lifetimes:
- `Lifetime.Transient`
- `Lifetime.PerContainer`
- `Lifetime.PerScope`  

The `Lifetime.Transient` is the default setting which means singularity will return a new instance everytime a instance is requested. For more info see [Lifetime](~/api/Singularity.Lifetime.yml)

The following example illustrates how lifetimes can be used:
```cs
var container = new Container(builder =>
{
    builder.Register<IScopedService, ScopedService>().With(Lifetime.PerScope));
	builder.Register<ISingleton, Singleton>().With(Lifetime.Singleton));
});

Scoped scope1 = container.BeginScope();

//service1 and service2 will point to the same instance due to the PerScope lifetime
var service1 = scope1.GetInstance<IScopedService>();
var service2 = scope1.GetInstance<IScopedService>();

var singleton1 = scope1.GetInstance<ISingleton>();

Scoped scope2 = container.BeginScope();
//service3 and service4 will point to the same instance but this instance is not the same as the one that service1 and service2 point to.
//This is because we are requesting these instances in a new scope and the services are registered as PerScope
var service3 = scope2.GetInstance<IScopedService>();
var service4 = scope2.GetInstance<IScopedService>();

//singleton1 and singleton2 will point to the same instance due to the Singleton lifetime
var singleton2 = scope2.GetInstance<ISingleton>();
```