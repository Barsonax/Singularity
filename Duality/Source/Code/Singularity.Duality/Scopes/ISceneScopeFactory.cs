using Duality.Resources;

namespace Singularity.Duality.Scopes
{
	internal interface ISceneScopeFactory
	{
		SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider, ILogger logger);
	}
}