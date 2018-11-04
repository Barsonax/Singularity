using Duality.Resources;
using Singularity.Exceptions;

namespace Singularity.Duality.Scopes
{
	public interface ISceneScopeFactory
	{
		SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider, ILogger logger);
	}
}