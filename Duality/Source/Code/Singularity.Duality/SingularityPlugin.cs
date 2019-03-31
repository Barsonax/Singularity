using System.Collections.Generic;
using System.Linq;
using Duality;
using Singularity.Duality.Resources;
using Singularity.Duality.Scopes;

namespace Singularity.Duality
{
    /// <summary>
    /// The singularity plugin that provides integration with the duality game engine.
    /// </summary>
	public class SingularityPlugin : CorePlugin
	{
		private GameScope? _gameScope;

#pragma warning disable 1591
        protected override void OnGameStarting()
#pragma warning restore 1591
        {
			var logger = new LoggerAdapter(Logs.Game);
			IEnumerable<SingularityModules> moduleResources = ContentProvider.GetAvailableContent<SingularityModules>().Select(x => x.Res);
			_gameScope = new GameScope(logger, new SceneScopeFactory(), new SceneEventsProvider(), moduleResources);
		}

#pragma warning disable 1591
        protected override void OnGameEnded()
#pragma warning restore 1591
        {
			_gameScope?.Dispose();
		}
	}
}
