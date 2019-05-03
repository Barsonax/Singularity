## Configuring with module components
When using `Singularity.Duality` the framework already creates a container for you. You only have to configure the dependencies. Currently there are 2 scopes in duality for dependencies:
* [Game](https://github.com/Barsonax/Singularity/wiki/Game-Scope-Dependencies), for dependencies living the entire game
* [Scene](https://github.com/Barsonax/Singularity/wiki/Scene-Scope-Dependencies), for dependencies that only live for a single scene

The dependencies are registered in a class that implements the `IModule` interface. The interface itself is explained in more detail [here](https://github.com/Barsonax/Singularity/wiki/Modules). Each scope has its own way of defining the modules themselves.

## How are dependencies injected?
Ok you now know how to configure your dependencies but how do you actually get them in the components? Duality already creates the components so unfortunately we cannot use constructor injection here. Instead we have to resort to method injection. In order to use method you need is a component with a method thats marked with the `[Inject]` attribute:

```cs
public class ExampleComponent : Component
{
	public IGameManager GameManager { get; private set; }
	public IPathfinder Pathfinder { get; private set; }
	
	[Inject]
	public void Init(IGameManager gameManager, IPathfinder pathfinder)
	{
		GameManager = gameManager;
		Pathfinder = pathfinder;
	}
}
```

`Singularity.Duality` with then call those methods whenever a component is activated.

More info about method injection can be found in this [article](https://github.com/Barsonax/Singularity/wiki/Method-injection).