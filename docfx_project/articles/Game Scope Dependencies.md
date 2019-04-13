Game dependencies will live as long as the game is running. This means they will survive between scenes. This functionality is made possible through the use of [Nested containers](https://github.com/Barsonax/Singularity/wiki/Nested-containers).

When the starting a game `Singularity.Duality` will search for resources of type `SingularityModules`. These resources contains a array of `ModuleRef` instances that contain the data needed to create the modules:
* `Assembly`, this is the name of the dll but without the .dll at the end.
* `NameSpace`, this is the namespace where the module resides in
* `Name`, this is the name of the module class

If everything was typed in correctly then the `Type` property will be filled in. Also if `Singularity.Duality` was not able to instantiate the module when the game starts it will log a warning explaining why.

A example of a game dependency module can be found below:
```cs
public class GameDependencies : IModule
{
	public void Register(BindingConfig config)
	{
		config.Register<IPathfinder, Pathfinder>();
	}
}
```