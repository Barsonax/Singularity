using System.Collections.Generic;
using Duality.Resources;
using Singularity.Duality.Scopes;

namespace Singularity.Duality.Test
{
	internal class SceneScopeFactoryMockup : ISceneScopeFactory
	{
		public readonly List<(GameScope gameScope, Scene scene)> CreateCalls = new List<(GameScope gameScope, Scene scene)>();
		public readonly List<SceneScope> CreatedSceneScopes = new List<SceneScope>();

		public SceneScope Create(GameScope gameScope, Scene scene, ISceneEventsProvider sceneEventsProvider, ILogger logger)
		{
			CreateCalls.Add((gameScope, scene));
			var sceneScope = new SceneScope(gameScope.Container, scene, sceneEventsProvider, logger);
			CreatedSceneScopes.Add(sceneScope);
			return sceneScope;
		}
	}
}
