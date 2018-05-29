using Duality;
using Singularity.Bindings;

namespace Singularity.Duality.Examples.Components
{
    public class GameDependenciesComponent : Component, IModule
    {
	    public void Register(BindingConfig bindingConfig)
	    {
		    bindingConfig.For<IPathfinder>().Inject<Pathfinder>();
			bindingConfig.For<IGameManager>().Inject(() => GameObj.ParentScene.FindComponent<GameManagerComponent>(true)).With(Lifetime.PerContainer);
	    }
    }
}
