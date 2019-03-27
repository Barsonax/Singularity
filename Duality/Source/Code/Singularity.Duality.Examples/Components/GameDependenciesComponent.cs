using Duality;

namespace Singularity.Duality.Examples.Components
{
    public class GameDependenciesComponent : Component, IModule
    {
	    public void Register(BindingConfig bindingConfig)
	    {
            bindingConfig.Register<IPathfinder, Pathfinder>();
			bindingConfig.Register<IGameManager>().Inject(() => GameObj.Scene.FindComponent<GameManagerComponent>(true)).With(CreationMode.Singleton);
	    }
    }
}
