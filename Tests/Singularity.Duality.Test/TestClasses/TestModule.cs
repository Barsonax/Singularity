namespace Singularity.Duality.Test
{
	public class TestModule : IModule
	{
		public void Register(ContainerBuilder config)
		{
			config.Register<IModule, TestModule>();
		}
	}
}