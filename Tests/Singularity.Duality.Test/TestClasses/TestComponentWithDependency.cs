using System.Collections.Generic;
using Duality;

using Singularity.Attributes;
using Singularity.Bindings;

namespace Singularity.Duality.Test
{
	public class TestComponentWithDependency : Component
	{
		public IModule Module { get; private set; }
	    public List<IModule> InitCalls { get; } = new List<IModule>();

		[Inject]
		public void Init(IModule module)
		{
			Module = module;
            InitCalls.Add(module);
		}
	}
}