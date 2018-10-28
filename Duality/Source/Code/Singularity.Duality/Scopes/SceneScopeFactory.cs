using Duality.Resources;

namespace Singularity.Duality.Scopes
{
	public class SceneScopeFactory : ISceneScopeFactory
	{
		public SceneScope Create(GameScope gameScope, Scene scene)
		{
			return new SceneScope(gameScope.Container, Scene.Current);
		}
	}
}