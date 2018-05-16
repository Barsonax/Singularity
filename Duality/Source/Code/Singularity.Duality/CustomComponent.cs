using Duality;
using Singularity.Bindings;

namespace Singularity.Duality
{
	public class CustomComponent : Component, ICmpInitializable
	{
		[Inject]
		public void Initialize(IDummyDependency dummyDependency)
		{

		}

		public void OnInit(InitContext context)
		{
			
		}

		public void OnShutdown(ShutdownContext context)
		{
			
		}
	}

	public class DummyDependency : IDummyDependency
	{

	}

	public interface IDummyDependency
	{

	}

	public class DependencyComponent : Component, IModule
	{
		public void Register(BindingConfig bindingConfig)
		{
			bindingConfig.For<IDummyDependency>().Inject<DummyDependency>();
		}
	}
}
