using Duality.Resources;
using Singularity.Exceptions;

namespace Singularity.Duality.Scopes
{
	public class SceneScopeFactory : ISceneScopeFactory
	{
		public SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider, ILogger logger)
		{
			return new SceneScope(gameScope.Container, scene, sceneEventsProvider, logger);
		}
	}
}