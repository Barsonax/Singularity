using Duality.Resources;

namespace Singularity.Duality.Scopes
{
	public class SceneScopeFactory : ISceneScopeFactory
	{
		public SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider)
		{
			return new SceneScope(gameScope.Container, scene, sceneEventsProvider);
		}
	}
}