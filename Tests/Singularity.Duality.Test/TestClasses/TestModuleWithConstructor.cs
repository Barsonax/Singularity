using Singularity.Bindings;

namespace Singularity.Duality.Test
{
	public class TestModuleWithConstructor : IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			
		}

		public TestModuleWithConstructor(int value)
		{

		}
	}
}