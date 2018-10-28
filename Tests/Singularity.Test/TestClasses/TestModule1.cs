using Singularity.Bindings;

namespace Singularity.Test.TestClasses
{
	public class TestModule1 : IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			bindingConfig.For<ITestService10>().Inject<TestService10>();
		}
	}
}