namespace Singularity.Duality.Examples.Components
{
	public class ExampleGameDependency : IModule
	{
		public void Register(BindingConfig config)
		{
			config.Register<object, Pathfinder>();
		}
	}
}
