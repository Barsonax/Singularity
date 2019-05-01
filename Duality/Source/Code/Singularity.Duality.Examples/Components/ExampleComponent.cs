using Duality;


namespace Singularity.Duality.Examples.Components
{
    public class ExampleComponent : Component
    {
		public IGameManager GameManager { get; private set; }
		public IPathfinder Pathfinder { get; private set; }

	    public void Init(IGameManager gameManager, IPathfinder pathfinder)
		{
			GameManager = gameManager;
			Pathfinder = pathfinder;
		}
    }
}
