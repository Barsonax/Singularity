using Duality.Resources;
using System;

namespace Singularity.Duality.Scopes
{
    internal sealed class SceneScopeFactory : ISceneScopeFactory
	{
	    public SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider, ILogger logger)
		{
            if (gameScope.Container == null) throw new ArgumentNullException($"{nameof(gameScope)}.{nameof(gameScope.Container)}");
			return new SceneScope(gameScope.Container, scene, sceneEventsProvider, logger);
		}
	}
}