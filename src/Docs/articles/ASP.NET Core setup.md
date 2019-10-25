## ASP.NET Core 3.0
Setup in ASP.NET Core 3.0 is very simple, just call `UseServiceProviderFactory` and pass in a `SingularityServiceProviderFactory` instance:
```cs
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseServiceProviderFactory(new SingularityServiceProviderFactory());
}
```

If you want to register dependencies using the Singularity API just add this in the startup.cs:
```cs
public void ConfigureContainer(ContainerBuilder builder)
{
    //Register services using singularities ContainerBuilder here
}
```