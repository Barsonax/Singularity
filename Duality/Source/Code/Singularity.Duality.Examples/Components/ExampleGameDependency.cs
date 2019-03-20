using Singularity.Bindings;

namespace Singularity.Duality.Examples.Components
{
	public class ExampleGameDependency : IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			bindingConfig.Register<object, Pathfinder>();
		}
	}
}
