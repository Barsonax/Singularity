namespace Singularity.Duality.Test
{
	public class TestModule : IModule
	{
		public void Register(BindingConfig config)
		{
			config.Register<IModule, TestModule>();
		}
	}
}