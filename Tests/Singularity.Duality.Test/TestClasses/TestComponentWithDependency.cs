using Duality;
using Singularity.Bindings;

namespace Singularity.Duality.Test
{
	public class TestComponentWithDependency : Component
	{
		public IModule Module { get; private set; }

		[Inject]
		public void Init(IModule module)
		{
			Module = module;
		}
	}
}