using Duality;
using Singularity.Duality.Resources;
using Singularity.Duality.Scopes;

namespace Singularity.Duality
{
	public class SingularityPlugin : CorePlugin
	{
		private GameScope _gameScope;

		protected override void OnGameStarting()
		{
			var logger = new LoggerAdapter(Log.Game);
			var moduleResources = ContentProvider.GetAvailableContent<SingularityModules>();
			_gameScope = new GameScope(logger, new SceneScopeFactory(), moduleResources);
		}

		protected override void OnGameEnded()
		{
			_gameScope?.Dispose();
		}
	}
}
