## ASP.NET Core 3.0
Setup in ASP.NET Core 3.0 is very simple. First reference the `Singularity.AspNetCore.Hosting` package. Then in your Program.cs add the `Singularity` namespace and call `UseSingularity` on the `IHostBuilder`
```cs
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseSingularity();
}
```

At this point your application will already work but for better integration you can reference the `Singularity.AspNetCore.MVC` package. Then in your Startup.cs add the `Singularity` namespace and call `SetupMvc` in the `ConfigureContainer` method. This will add custom Controller and View activators to the container. You can also do additional configurations here such as enabling a logger. Finally this is also the place where you can register dependencies using Singularities API:
```cs
public void ConfigureContainer(ContainerBuilder builder)
{
    //Register services using singularities ContainerBuilder here
    builder.SetupMvc();
    builder.ConfigureSettings(s =>
    {
        s.With(Loggers.ConsoleLogger);
    });
    builder.Register<ITransient1, Transient1>();
}
```