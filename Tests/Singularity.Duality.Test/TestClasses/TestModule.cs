using Singularity.Bindings;

namespace Singularity.Duality.Test
{
	public class TestModule : IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			bindingConfig.For<IModule>().Inject<TestModule>();
		}
	}
}