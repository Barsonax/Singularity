namespace Singularity.Duality.Examples.Components
{
	public class ExampleGameDependency : IModule
	{
		public void Register(ContainerBuilder config)
		{
			config.Register<object, Pathfinder>();
		}
	}
}
