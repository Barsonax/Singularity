using Duality.Resources;

namespace Singularity.Duality.Scopes
{
    internal sealed class SceneScopeFactory : ISceneScopeFactory
	{
	    public SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider, ILogger logger)
		{
			return new SceneScope(gameScope.Container, scene, sceneEventsProvider, logger);
		}
	}
}