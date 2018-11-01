using Duality.Resources;

namespace Singularity.Duality.Scopes
{
	public interface ISceneScopeFactory
	{
		SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider);
	}
}