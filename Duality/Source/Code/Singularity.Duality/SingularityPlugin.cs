using Duality;
using Singularity.Duality.Scopes;

namespace Singularity.Duality
{
	public class SingularityPlugin : CorePlugin
	{
		private GameScope _gameScope;

		protected override void OnGameStarting()
		{
			_gameScope = new GameScope();		
		}

		protected override void OnGameEnded()
		{			
			_gameScope.Dispose();
		}
	}
}
