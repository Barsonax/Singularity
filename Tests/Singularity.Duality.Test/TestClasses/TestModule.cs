namespace Singularity.Duality.Test
{
	public class TestModule : IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			bindingConfig.Register<IModule, TestModule>();
		}
	}
}