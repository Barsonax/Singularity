namespace Singularity.Duality.Test
{
	public class TestModuleWithConstructor : IModule
	{
		public void Register(ContainerBuilder config)
		{

		}

		public TestModuleWithConstructor(int value)
		{

		}
	}
}