using Singularity.Bindings;

namespace Singularity.Duality.Examples.Components
{
	public class ExampleGameDependency : IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			bindingConfig.For<object>().Inject<Pathfinder>();
		}
	}
}
