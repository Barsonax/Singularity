When the entering a new scene `Singularity.Duality` will search the entire scene for any components implementing `IModule`. It will then call `Register` on all those components in order to get the dependencies. A example component that implements `IModule` can be found below:
```cs
public class SceneDependenciesComponent : Component, IModule
{
	public void Register(BindingConfig config)
	{
		config.For<IGameManager>().Inject(() => GameObj.ParentScene.FindComponent<GameManagerComponent>(true)).With(Lifetime.PerContainer);
	}
}
```

It will then create a [Nested container](https://github.com/Barsonax/Singularity/wiki/Nested-containers) with the registered scene dependencies. This means any dependencies that are defined as [Game Scope Dependencies](https://github.com/Barsonax/Singularity/wiki/Game-Scope-Dependencies) will also be available.