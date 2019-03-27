namespace Singularity.TestClasses.TestClasses
{
	public class TestModule1 : IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			bindingConfig.Register<ITestService10, TestService10>();
		}
	}
}