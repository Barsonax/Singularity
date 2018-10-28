using System.Collections.Generic;
using Duality.Resources;
using Singularity.Duality.Scopes;

namespace Singularity.Duality.Test
{
	public class SceneScopeFactoryMockup : ISceneScopeFactory
	{
		public readonly List<(GameScope gameScope, Scene scene)> CreateCalls = new List<(GameScope gameScope, Scene scene)>();
		public readonly List<SceneScope> CreatedSceneScopes = new List<SceneScope>();

		public SceneScope Create(GameScope gameScope, Scene scene)
		{
			CreateCalls.Add((gameScope, scene));
			var sceneScope = new SceneScope(gameScope.Container, scene);
			CreatedSceneScopes.Add(sceneScope);
			return sceneScope;
		}
	}
}
