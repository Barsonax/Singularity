using System.Collections.Generic;
using System.Linq;
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
			IEnumerable<SingularityModules> moduleResources = ContentProvider.GetAvailableContent<SingularityModules>().Select(x => x.Res);
			_gameScope = new GameScope(logger, new SceneScopeFactory(), new SceneEventsProvider(), moduleResources);
		}

		protected override void OnGameEnded()
		{
			_gameScope?.Dispose();
		}
	}
}
