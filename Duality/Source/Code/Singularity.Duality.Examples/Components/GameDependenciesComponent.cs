using Duality;

namespace Singularity.Duality.Examples.Components
{
    public class GameDependenciesComponent : Component, IModule
    {
	    public void Register(BindingConfig config)
	    {
            config.Register<IPathfinder, Pathfinder>();
			config.Register<IGameManager>().Inject(() => GameObj.Scene.FindComponent<GameManagerComponent>(true)).With(Lifetime.PerContainer);
	    }
    }
}
