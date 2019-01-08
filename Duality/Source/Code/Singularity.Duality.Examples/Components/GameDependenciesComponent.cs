using Duality;
using Singularity.Bindings;
using Singularity.Enums;

namespace Singularity.Duality.Examples.Components
{
    public class GameDependenciesComponent : Component, IModule
    {
	    public void Register(BindingConfig bindingConfig)
	    {
		    bindingConfig.For<IPathfinder>().Inject<Pathfinder>();
			bindingConfig.For<IGameManager>().Inject(() => GameObj.Scene.FindComponent<GameManagerComponent>(true)).With(Lifetime.PerContainer);
	    }
    }
}
