namespace Singularity.TestClasses.TestClasses
{
	public class TestModule1 : IModule
	{
		public void Register(ContainerBuilder containerBuilder)
		{
			containerBuilder.Register<ITestService10, TestService10>();
		}
	}
}