## ServiceBindingGenerators
All of the advanced functionality in Singularity such as open generics and collection support is provided by ServiceBindingGenerators. 

By default all of the built in binding generators are enabled but if you want to disable some or add your own you can use the SingularitySettings API to do so. For instance in the following example we will disable resolving concrete types:
```cs
var container = new Container(containerBuilder =>
{
    containerBuilder.ConfigureSettings(settings =>
    {
        settings.ConfigureServiceBindingGenerators(generators =>
        {
            generators.Remove(x => x is ConcreteServiceBindingGenerator);
        });
    });
});
```

To make your own generator you need to implement the `IServiceBindingGenerator` interface:
```cs
public class CustomServiceBindingGenerator : IServiceBindingGenerator
{
    public IEnumerable<ServiceBinding> Resolve(IInstanceFactoryResolver resolver, Type type)
    {
        //create and yield return your bindings here
        //if no elements are returned the next IServiceBindingGenerator will be used.
    }
}
```
To use it you have to add it to the settings:
```cs
var container = new Container(containerBuilder =>
{
    containerBuilder.ConfigureSettings(settings =>
    {
        settings.ConfigureServiceBindingGenerators(generators =>
        {
            generators.Add(new CustomServiceBindingGenerator());
        });
    });
});
```
Sometimes the order of the generators can be important. For instance we want to use our custom generator before the CollectionServiceBindingGenerator:
```cs
var container = new Container(containerBuilder =>
{
    containerBuilder.ConfigureSettings(settings =>
    {
        settings.ConfigureServiceBindingGenerators(generators =>
        {
            generators.Before(x => x is CollectionServiceBindingGenerator, new CustomServiceBindingGenerator());
        });
    });
});
```
Note that in the case of the Before and After methods if no match is found a exception will be thrown.