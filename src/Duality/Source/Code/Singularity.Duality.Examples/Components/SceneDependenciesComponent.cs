using Duality;

namespace Singularity.Duality.Examples.Components
{
    public class SceneDependenciesComponent : Component, IModule
    {
	    public void Register(ContainerBuilder config)
	    {
            config.Register<IPathfinder, Pathfinder>();
            config.Register<IGameManager>(c => c
                .Inject(() => GameObj.Scene.FindComponent<GameManagerComponent>(true))
                .With(Lifetimes.PerContainer));
	    }
    }
}
